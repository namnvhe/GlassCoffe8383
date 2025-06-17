namespace Cafe.BusinessObjects.Models.Response
{
    public class ForgotPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ResetToken { get; set; }
        public DateTime? TokenExpiryTime { get; set; }
        public List<string>? Errors { get; set; }
    }
}
