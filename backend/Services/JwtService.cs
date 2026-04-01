using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RightFitGigs.Models;

namespace RightFitGigs.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string _issuer = "rightfitgigs";
        private readonly string _audience = "rightfitgigs";
        private readonly int _expiryDays = 7;

        public JwtService(IConfiguration config)
        {
            // Read from environment variable or appsettings; dev fallback is long enough to be secure for local use only
            _secret = config["JWT_SECRET"]
                ?? "RFG_Dev_Only_Secret_Key_Must_Be_At_Least_32_Characters_Long!";
        }

        /// <summary>
        /// Issues a signed JWT for the given user. Expires in 7 days.
        /// </summary>
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userType", user.UserType),
                new Claim("isAdmin", user.IsAdmin.ToString().ToLower()),
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_expiryDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Returns the TokenValidationParameters used by both the middleware and manual checks.
        /// </summary>
        public TokenValidationParameters GetValidationParameters() => new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <summary>
    /// Extension helpers for reading claims off a ClaimsPrincipal.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        public static bool GetIsAdmin(this ClaimsPrincipal principal)
            => principal.FindFirstValue("isAdmin") == "true";

        public static string? GetUserType(this ClaimsPrincipal principal)
            => principal.FindFirstValue("userType");
    }
}
