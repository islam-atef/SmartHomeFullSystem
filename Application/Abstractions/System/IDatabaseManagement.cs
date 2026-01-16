using Application.Contracts.System;
using System.Threading.Tasks;

namespace Application.Abstractions.System
{
    public interface IDatabaseManagement
    {
        /// <summary>
        /// Backs up the current database to the specified path or a default location.
        /// </summary>
        /// <param name="backupPath">Optional absolute path to the directory where the backup should be stored.</param>
        /// <param name="databaseName">Optional database name. If null, uses the current database.</param>
        /// <returns>The full path of the created backup file.</returns>
        Task<string> BackupDatabaseAsync(string? backupPath = null, string? databaseName = null);

        /// <summary>
        /// Restores the database from a specified backup file.
        /// WARNING: This overwrites the current database.
        /// </summary>
        /// <param name="backupFilePath">Absolute path to the .bak file.</param>
        /// <param name="databaseName">Optional database name.</param>
        Task RestoreDatabaseAsync(string backupFilePath, string? databaseName = null);

        /// <summary>
        /// Retrieves a list of available database backups.
        /// </summary>
        Task<List<BackupInfo>> GetBackupsAsync(string? backupPath = null, string? databaseName = null);
    }
}
