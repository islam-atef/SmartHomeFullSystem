using Application.Abstractions.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment when Identity is fully configured and you want to restrict this
    public class DatabaseManagementController : ControllerBase
    {
        private readonly IDatabaseManagement _databaseManagement;

        public DatabaseManagementController(IDatabaseManagement databaseManagement)
        {
            _databaseManagement = databaseManagement;
        }

        [HttpPost("backup")]
        public async Task<IActionResult> CreateBackup([FromQuery] string? backupPath = null)
        {
            try
            {
                var fullPath = await _databaseManagement.BackupDatabaseAsync(backupPath);
                return Ok(new { message = "Backup created successfully", path = fullPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Backup failed", error = ex.Message });
            }
        }

        [HttpPost("restore")]
        public async Task<IActionResult> RestoreDatabase([FromQuery] string backupPath)
        {
            try
            {
                await _databaseManagement.RestoreDatabaseAsync(backupPath);
                return Ok(new { message = "Database restored successfully. Applications flows may have been reset." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Restore failed. Be careful, the database might be in an inconsistent state.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBackups()
        {
            try
            {
                var backups = await _databaseManagement.GetBackupsAsync();
                return Ok(backups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve backups", error = ex.Message });
            }
        }
    }
}
