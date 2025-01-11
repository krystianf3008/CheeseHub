using CheeseHub.Data;
using CheeseHub.Extensions;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.RefreshToken;
using CheeseHub.Models.User;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace CheeseHub.Services
{
    public class JwtService : IJwtService
    {
        public IUserService _userService { get; set; }
        private readonly AuthenticationSettings _authenticationSettings;
        ApplicationDbContext ApplicationDbContext { get; set; }

        public JwtService(IUserService userService, AuthenticationSettings authenticationSettings, ApplicationDbContext applicationDbContext) 
        { 
            _userService = userService;
            _authenticationSettings = authenticationSettings;
            ApplicationDbContext = applicationDbContext;


        }
        public async Task<(string JwtToken, string RefreshToken)> GenerateTokens(Guid userId)
        {
            var user = await _userService.GetWithRole(id: userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Name)
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Key));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _authenticationSettings.Issuer,
                _authenticationSettings.Issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(_authenticationSettings.ExpireHours),
                signingCredentials: credentials
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            string refreshToken = Guid.NewGuid().ToString();
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_authenticationSettings.RefreshExpireDays),
                CreatedAt = DateTime.UtcNow
            };

            ApplicationDbContext.RefreshTokens.Add(newRefreshToken);
            await ApplicationDbContext.SaveChangesAsync();

            return (jwt, refreshToken);
        }
        public async Task<bool> ValidateRefreshToken(Guid userId, string refreshToken)
        {
            var token =  ApplicationDbContext.RefreshTokens
                .FirstOrDefault(t => t.UserId == userId && t.Token == refreshToken);

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = ApplicationDbContext.RefreshTokens.Where(t => t.ExpiryDate < DateTime.UtcNow);
            ApplicationDbContext.RefreshTokens.RemoveRange(expiredTokens);
            await ApplicationDbContext.SaveChangesAsync();
        }
        public async Task RevokeRefreshToken(string refreshToken)
        {
            var token = ApplicationDbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token != null)
            {
                ApplicationDbContext.RefreshTokens.Remove(token);
                await ApplicationDbContext.SaveChangesAsync();
            }
        }

    }
}
