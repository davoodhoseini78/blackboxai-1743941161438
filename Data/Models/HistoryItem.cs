using System;

namespace YourNamespace.Data.Models
{
    public class HistoryItem
    {
        public string FileName { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; }
    }
}