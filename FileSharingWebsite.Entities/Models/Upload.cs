namespace FileSharingWebsite.Entities.Models
{
    public class Upload
    {
        public int UploadId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string Path { get; set; } = string.Empty;
        public int Counter { get; set; }
    }
}
