using System;

namespace YourNamespace.Data.Models
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; }
        public string DestinationPath { get; set; }
        public string Checksum { get; set; }
    }
}