using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Services
{
    public class JwtConfigurationService
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string SecretKey => _configuration["JWT:Secret"];
        public static string Issuer => _configuration["JWT:Issuer"];
        public static string Audience => _configuration["JWT:Audience"];
        public static int ExpiryMinutes => int.TryParse(_configuration["JWT:ExpiryMinutes"], out int minutes) ? minutes : 15;
    }
}
