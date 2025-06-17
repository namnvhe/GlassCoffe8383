namespace Cafe.BusinessObjects.Models.Response
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}