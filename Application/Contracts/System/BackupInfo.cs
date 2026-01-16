using System;

namespace Application.Contracts.System
{
    public class BackupInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
