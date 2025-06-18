namespace Cafe.BusinessObjects.Models.Response
{
    public class ResetPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}
