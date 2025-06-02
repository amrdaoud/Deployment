

using AccountLib.Configuration;
using AccountLib.Contracts;
using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts.JWT.Response;
using AccountLib.Data;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountLib.Services.IdentityAccountService
{
	public class IdentityAccountService(
										ApplicationDbContext db,
										UserManager<ApplicationUser> userManager,
										RoleManager<ApplicationRole> roleManager,
										IConfiguration configuration,
										IOptions<JwtSettings> jwtOptions) : IIdentityAccountService
	{

		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly IConfiguration _config = configuration;
		private readonly ApplicationDbContext _db = db;
		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
		private readonly JwtSettings _jwtSettings = jwtOptions.Value;


		private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
		{
			var userRoles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>{
				new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
				new(JwtRegisteredClaimNames.Email, user.Email ?? "")
			};

			claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
			issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
			claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}








		public async Task<ResultWithMessage> RegisterAsync(RegisterRequest request)
		{
			var user = new ApplicationUser
			{
				UserName = request.UserName,
				Email = request.Email,
				EmailConfirmed = true,
				NormalizedEmail = request.Email.ToUpper(),
				NormalizedUserName = request.UserName.ToUpper(),
				PhoneNumber = request.PhoneNumber
			};

			var result = await _userManager.CreateAsync(user, request.Password);

			if (!result.Succeeded)
				return new ResultWithMessage(null, result.Errors.First().Description);

			foreach (var tenantRole in request.TenantRoles)
			{
				foreach (var roleId in tenantRole.RoleIds)
				{
					var role = await _roleManager.FindByIdAsync(roleId);

					if (role == null)
						continue;

					_db.UserRoles.Add(new ApplicationUserTenantRole
					{
						UserId = user.Id,
						RoleId = Guid.Parse(roleId),
						TenantId = Guid.Parse(tenantRole.TenantId),
					});
				}
			}

			await _db.SaveChangesAsync();

			return new ResultWithMessage(null, string.Empty);
		}
		public async Task<ResultWithMessage> LoginAsync(LoginRequest request)
		{

			var loginResult = new LoginResponse
			{
				IsAuthenticated = false
			};

			var user = await _userManager.FindByEmailAsync(request.EmailOrUsername) ??
				  await _userManager.FindByNameAsync(request.EmailOrUsername);

			if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
				return new ResultWithMessage(loginResult, "Invalid credentials");

			var token = await GenerateJwtTokenAsync(user);

			loginResult = new LoginResponse
			{
				UserName = user.UserName,
				Email = user.Email,
				IsAuthenticated = true,
				Token = token,
				TokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
			};

			return new ResultWithMessage(loginResult, string.Empty);
		}
	}
}
