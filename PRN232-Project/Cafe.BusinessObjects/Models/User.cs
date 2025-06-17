using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiry { get; set; }

    public string Role { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool IsEmailVerified { get; set; }

    public string? EmailVerificationToken { get; set; }

    public DateTime? EmailVerificationTokenExpiry { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
