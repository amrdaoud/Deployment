using AccountLib.Configuration;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccountLib.Services.IdentityAccountService.JwtProvider
{
	public class JwtProvider(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager) : IJwtProvider
	{
		private readonly JwtSettings _jwtSettings = jwtSettings.Value;
		private readonly UserManager<ApplicationUser> _userManager = userManager;


		public async Task<(string Token, DateTime ExpiryDate)> GenerateJwtTokenAsync(ApplicationUser user)
		{
			var userRoles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
				new(JwtRegisteredClaimNames.Email, user.Email ?? "")
			};

			claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: expiry,
				signingCredentials: creds
			);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return (jwt, expiry);
		}
		public (string Token, DateTime ExpiryDate) GenerateSecureToken()
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);

			return (Convert.ToBase64String(randomBytes), DateTime.UtcNow.AddDays(7)); // Refresh token valid for 7 days
		}
	}
}