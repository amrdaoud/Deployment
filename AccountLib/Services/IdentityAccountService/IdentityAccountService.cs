using AccountLib.Configuration;
using AccountLib.Contracts;
using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts.JWT.Response;
using AccountLib.Data;
using AccountLib.Errors;
using AccountLib.Models;
using AccountLib.Services.EmailSenderService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
										IOptions<JwtSettings> jwtOptions,
										IEmailSender emailSender) : IIdentityAccountService
	{

		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly ApplicationDbContext _db = db;
		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
		private readonly JwtSettings _jwtSettings = jwtOptions.Value;
		private readonly IEmailSender _emailSender = emailSender;


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
				return new ResultWithMessage(loginResult, IdentityAccountErrors.InvalidCredentials);

			var token = await GenerateJwtTokenAsync(user);

			var userRoles = await _userManager.GetRolesAsync(user);

			var userTenants = await _db.UserTenantRoles
								.Include(utr => utr.Tenant)
								.Where(utr => utr.UserId == user.Id)
								.Select(utr => utr.Tenant!.Name)
								.Distinct()
								.ToListAsync();

			loginResult = new LoginResponse
			{
				UserName = user.UserName,
				Email = user.Email,
				IsAuthenticated = true,
				Token = token,
				Roles = [.. userRoles],
				Tenants = userTenants!,
				TokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
			};

			return new ResultWithMessage(loginResult, string.Empty);
		}
		public async Task<ResultWithMessage> SendResetPasswordEmailAsync(SendResetPasswordEmailRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null)
				return new ResultWithMessage(null, IdentityAccountErrors.UserNotFound);

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedToken = Uri.EscapeDataString(token);
			var resetLink = $"{request.BaseUrl}?email={request.Email}&token={encodedToken}";

			var subject = "Password Reset";
			var body = $"Dear, <br> Please click to reset password: <a href='{resetLink}'>{resetLink}</a>";

			try
			{
				await _emailSender.SendEmailAsync(request.Email, subject, body);
			}
			catch (Exception ex)
			{
				return new ResultWithMessage(null, ex.Message);
			}

			return new ResultWithMessage(null, string.Empty);
		}
		public async Task<ResultWithMessage> ResetPasswordAsync(ResetPasswordRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null)
				return new ResultWithMessage(null, IdentityAccountErrors.InvalidEmail);

			try
			{
				var decodedToken = Uri.UnescapeDataString(request.Token);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
			}
			catch (Exception ex)
			{
				return new ResultWithMessage(null, ex.Message);
			}

			return new ResultWithMessage(null, string.Empty);
		}
		public async Task<ResultWithMessage> ChangePasswordAsync(ChangePasswordRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null)
				return new ResultWithMessage(null, IdentityAccountErrors.UserNotFound);

			var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

			if (!result.Succeeded)
				return new ResultWithMessage(null, result.Errors.First().Description);

			return new ResultWithMessage(null, string.Empty);
		}
	}
}