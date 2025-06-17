using Cafe.BusinessObjects.Models;
using Cafe.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class JwtToken
    {
        // Tạo Access Token
        public static string GenerateAccessToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(JwtConfigurationService.SecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("IsEmailVerified", user.IsEmailVerified.ToString())
                    }),
                    Expires = DateTime.Now.AddMinutes(JwtConfigurationService.ExpiryMinutes),
                    Issuer = JwtConfigurationService.Issuer,
                    Audience = JwtConfigurationService.Audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw new Exception($"Error generating access token: {e.Message}");
            }
        }

        // Tạo Refresh Token
        public static string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error generating refresh token: {e.Message}");
            }
        }

        // Xác thực Access Token
        public static ClaimsPrincipal ValidateAccessToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(JwtConfigurationService.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = JwtConfigurationService.Issuer,
                    ValidateAudience = true,
                    ValidAudience = JwtConfigurationService.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Lấy thông tin user từ token
        public static int GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateAccessToken(token);
                if (principal != null)
                {
                    var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Kiểm tra token có hết hạn không
        public static bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.Now;
            }
            catch (Exception)
            {
                return true; // Nếu không đọc được token thì coi như hết hạn
            }
        }

        // Lấy thời gian hết hạn của token
        public static DateTime GetTokenExpiryTime(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}