using Application.Abstractions.System;
using Application.Contracts.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Persistence.Services.Background_Service
{
    public class ScheduledBackupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScheduledBackupService> _logger;

        // Set backup time to 3:00 AM
        private readonly TimeSpan _backupTime = default!;
        private readonly int _MaxBackupLastUpdate = default!;
        private readonly int _MaximumBackupDuration = default!;

        public ScheduledBackupService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ScheduledBackupService> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            // Set backup time to 3:00 AM

            int hour = (int.TryParse(_configuration["BackupSettings:Hour"], out int hourvalue))
                ? hourvalue : 3;
            _backupTime = new(hour,0,0);

            _MaxBackupLastUpdate = (int.TryParse(_configuration["BackupSettings:MaxBackupLastUpdate"], out int MBLUvalue))
                ? MBLUvalue : 2;

            _MaximumBackupDuration = (int.TryParse(_configuration["BackupSettings:MaximumBackupDuration"], out int MBDvalue))
                ? MBDvalue : 5;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledBackupService: ExecuteAsync: Scheduled Backup Service starting...");
            while (!stoppingToken.IsCancellationRequested)
            {
                var backupList =await GetBackupAsync();
                if (backupList == null || backupList.Count == 0)
                {
                    // Create a backup!
                    await PerformBackupAsync();
                }
                else if (backupList[0].CreatedAt.AddDays(_MaxBackupLastUpdate) < DateTime.Now) 
                {
                    // Create a new backup!
                    await PerformBackupAsync();
                }
                else
                {
                    var now = DateTime.Now;
                    var nextRun = now.Date.Add(_backupTime);

                    // If 3:00 AM already passed today, schedule for tomorrow
                    if (now > nextRun)
                    {
                        nextRun = nextRun.AddDays(1);
                    }
                    var delay = nextRun - now;
                    _logger.LogInformation($"ScheduledBackupService: ExecuteAsync: Next backup scheduled for: {nextRun}");
                    // Verify we don't wait -ve time (safety check)
                    if (delay.TotalMilliseconds > 0)
                    {
                        // Wait until 3:00 AM
                        await Task.Delay(delay, stoppingToken);
                    }
                    // Time to wake up and backup!
                    await PerformBackupAsync();
                }
            }
        }


        private async Task PerformBackupAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseManagement>();

                // Passing null uses the service's default logic (Backups folder in solution root)
                var path = await dbService.BackupDatabaseAsync();
                _logger.LogInformation($"ScheduledBackupService: PerformBackupAsync: Auto-Backup successful: {path}");

                // Cleanup old backups
                await DeleteOldBackupsAsync(dbService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScheduledBackupService: PerformBackupAsync: Auto-Backup FAILED.");
            }
        }

        private async Task DeleteOldBackupsAsync(IDatabaseManagement dbService)
        {
            try
            {
                // Passing null uses the service's default logic (Backups folder)
                var backups = await dbService.GetBackupsAsync();
                if (backups == null) return;

                if(backups.Count <= 2) return;

                foreach (var backup in backups)
                {
                    if (backup.CreatedAt < DateTime.Now.AddDays(-_MaximumBackupDuration))
                    {
                        try
                        {
                            if (File.Exists(backup.FullPath))
                            {
                                File.Delete(backup.FullPath);
                                _logger.LogInformation($"ScheduledBackupService: Deleted old backup: {backup.FileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"ScheduledBackupService: Failed to delete old backup: {backup.FileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScheduledBackupService: Cleanup failed.");
            }
        }

        private async Task<List<BackupInfo>?> GetBackupAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseManagement>();

                // Passing null uses the service's default logic (Backups folder)
                var backupInfos = await dbService.GetBackupsAsync();

                _logger.LogInformation("ScheduledBackupService: GetBackupAsync: Get-Backups successful");
                return backupInfos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScheduledBackupService: GetBackupAsync: Get-Backup FAILED.");
                return null;    
            }
        }
    }
}
