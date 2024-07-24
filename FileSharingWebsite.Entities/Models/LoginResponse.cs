namespace FileSharingWebsite.Entities.Models
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
