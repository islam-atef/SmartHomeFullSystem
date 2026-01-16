using Application.Abstractions.System;
using Microsoft.EntityFrameworkCore;
using Application.Contracts.System;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Infrastructure.Persistence.Services
{
    public class DatabaseManagementService : IDatabaseManagement
    {
        private readonly AppDbContext _context;

        public DatabaseManagementService(AppDbContext context)
        {
            _context = context;
        }



        public async Task<List<BackupInfo>> GetBackupsAsync(string? backupPath = null, string? databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = _context.Database.GetDbConnection().Database;
            }

            // Determine Path
            backupPath = EnsureBackupDirectory(backupPath);

            // 2. Get the names of the existing files.
            if (!Directory.Exists(backupPath)) return new List<BackupInfo>();

            var result = new List<BackupInfo>();
            var files = Directory.GetFiles(backupPath);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                // Simple filter: Check if it starts with DB name and ends with .bak
                if (!fileName.StartsWith(databaseName, StringComparison.OrdinalIgnoreCase) || !fileName.EndsWith(".bak"))
                    continue;

                // Attempt to parse DateTime from filename: DbName_yyyy-MM-dd_HH-mm-ss.bak
                // Split by '_' might be tricky if DbName has underscores. 
                // Better strategy: Extract the timestamp part.
                // Or just use File Creation Time for simplicity? User code tried to parse exact pattern.
                // Let's stick to FileInfo LastWriteTime as a fallback if parsing fails, or try regex.
                
                DateTime createdAt;
                try 
                {
                    // Pattern: {databaseName}_{yyyy-MM-dd_HH-mm-ss}.bak
                    // Remove extension
                    var nameNoExt = Path.GetFileNameWithoutExtension(fileName);
                    // Remove db prefix + 1 char for underscore
                    var timePart = nameNoExt.Substring(databaseName.Length + 1); 
                    createdAt = DateTime.ParseExact(timePart, "yyyy-MM-dd_HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    createdAt = File.GetCreationTime(file);
                }

                result.Add(new BackupInfo
                {
                    FileName = fileName,
                    FullPath = file,
                    CreatedAt = createdAt
                });
            }

            return result.OrderByDescending(x => x.CreatedAt).ToList();
        }



        public async Task<string> BackupDatabaseAsync(string? backupPath = null, string? databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = _context.Database.GetDbConnection().Database;
            }

            backupPath = EnsureBackupDirectory(backupPath);

            // 4. Generate Filename
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"{databaseName}_{timestamp}.bak";
            string fullPath = Path.Combine(backupPath, fileName);

            // 5. Construct SQL Command
            // Need absolute path for SQL Server
            var sqlFullPath = Path.GetFullPath(fullPath);

            var sqlCommand = $@"
                BACKUP DATABASE [{databaseName}] 
                TO DISK = '{sqlFullPath}' 
                WITH FORMAT, 
                MEDIANAME = 'SmartHomeBackups', 
                NAME = 'Full Backup of {databaseName}';";

            // 6. Execute
            await _context.Database.ExecuteSqlRawAsync(sqlCommand);

            return fullPath;
        }



        public async Task RestoreDatabaseAsync(string backupFilePath, string? databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = _context.Database.GetDbConnection().Database;
            }

            if (!File.Exists(backupFilePath))
            {
                throw new FileNotFoundException($"Backup file not found at: {backupFilePath}");
            }

            var currentConnString = _context.Database.GetDbConnection().ConnectionString;
            var masterConnString = ReplaceDatabaseNameInConnectionString(currentConnString, "master");

            using var connection = new SqlConnection(masterConnString);
            await connection.OpenAsync();

            var sql = $@"
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE [{databaseName}] FROM DISK = '{backupFilePath}' WITH REPLACE;
                    ALTER DATABASE [{databaseName}] SET MULTI_USER;
                    ";

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 300;
            await command.ExecuteNonQueryAsync();
        }



        private static string ReplaceDatabaseNameInConnectionString(string connectionString, string newDatabaseName)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = newDatabaseName
            };
            return builder.ConnectionString;
        }

        private string EnsureBackupDirectory(string? overridePath)
        {
            if (!string.IsNullOrEmpty(overridePath))
            {
                if (!Directory.Exists(overridePath)) Directory.CreateDirectory(overridePath);
                return overridePath;
            }

            // Default Logic
            string solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName 
                ?? Directory.GetCurrentDirectory();
            var path = Path.Combine(solutionRoot, "Backups");
            
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
