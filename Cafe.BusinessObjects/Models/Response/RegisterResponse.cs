﻿namespace Cafe.BusinessObjects.Models.Response
{
    public class RegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
        public string? EmailVerificationToken { get; set; }
        public List<string>? Errors { get; set; }
    }
}
