namespace Cafe.Fontend.Models
{
    public class ContactModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
    }

    public class UserInfoModel
    {
        public bool IsLoggedIn { get; set; } = false;
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
