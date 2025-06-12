using AccountLib.Contracts;
using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts.JWT.Response;
using AccountLib.Data;
using AccountLib.Errors;
using AccountLib.Models;
using AccountLib.Services.IdentityAccountService.JwtProvider;
using EmailSender.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountLib.Services.IdentityAccountService
{
	public class IdentityAccountService(
										ApplicationDbContext db,
										UserManager<ApplicationUser> userManager,
										RoleManager<ApplicationRole> roleManager,
										IJwtProvider jwtProvider,
										IEmailSenderService emailSenderService) : IIdentityAccountService
	{

		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly ApplicationDbContext _db = db;
		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
		private readonly IJwtProvider _jwtProvider = jwtProvider;
		private readonly IEmailSenderService _emailSenderService = emailSenderService;


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

			// Generate JWT Token
			var (jwtToken, jwtExpiry) = await _jwtProvider.GenerateJwtTokenAsync(user);

			// Generate Refresh Token
			var (refreshToken, refreshExpiry) = _jwtProvider.GenerateSecureToken();

			var refreshEntity = new RefreshToken
			{
				Token = refreshToken,
				ExpiryDate = refreshExpiry,
				UserId = user.Id
			};

			_db.RefreshTokens.Add(refreshEntity);
			await _db.SaveChangesAsync();

			// Get Roles and Tenants
			var userRoles = await _userManager.GetRolesAsync(user);

			var userTenants = await _db.UserTenantRoles
										.Include(utr => utr.Tenant)
										.Where(utr => utr.UserId == user.Id)
										.Select(utr => utr.Tenant!.Name)
										.Distinct()
										.ToListAsync();

			loginResult = new LoginResponse
			{
				UserName = user.UserName!,
				Email = user.Email!,
				IsAuthenticated = true,
				Token = jwtToken,
				TokenExpiry = jwtExpiry,
				RefreshToken = refreshToken,
				RefreshTokenExpiry = refreshExpiry,
				Roles = [.. userRoles],
				Tenants = userTenants!
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
				await _emailSenderService.SendEmailAsync(request.Email, subject, body);
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
		public async Task<ResultWithMessage> RefreshTokenAsync(string refreshToken)
		{
			var existingToken = await _db.RefreshTokens
			.Include(r => r.User)
			.ThenInclude(u => u.UserTenantRoles)
			.ThenInclude(utr => utr.Tenant)
			.FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);


			if (existingToken == null || existingToken.ExpiryDate < DateTime.UtcNow)
				return new ResultWithMessage(null, IdentityAccountErrors.InvalidRefreshToken);

			var user = existingToken.User!;
			var (Token, ExpiryDate) = await _jwtProvider.GenerateJwtTokenAsync(user);

			var (newRefreshToken, refreshExpiry) = _jwtProvider.GenerateSecureToken();

			var refreshTokenEntity = new RefreshToken
			{
				Id = Guid.NewGuid(),
				Token = newRefreshToken,
				ExpiryDate = refreshExpiry,
				UserId = user.Id
			};

			existingToken.IsRevoked = true;

			_db.RefreshTokens.Add(refreshTokenEntity);
			await _db.SaveChangesAsync();

			var result = new LoginResponse
			{
				Email = user.Email!,
				UserName = user.UserName!,
				IsAuthenticated = true,
				Token = Token,
				TokenExpiry = ExpiryDate,
				Roles = [.. (await _userManager.GetRolesAsync(user))],
				Tenants = user.UserTenantRoles.Select(x => x.Tenant?.Name).Where(x => x != null).Distinct().ToList()!,
				RefreshToken = newRefreshToken,
				RefreshTokenExpiry = refreshExpiry
			};

			return new ResultWithMessage(result, string.Empty);
		}
		public async Task<ResultWithMessage> LogoutAsync(string refreshToken)
		{
			var refreshTokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

			if (refreshTokenEntity == null)
				return new ResultWithMessage(null, IdentityAccountErrors.InvalidRefreshToken);

			_db.RefreshTokens.Remove(refreshTokenEntity);
			await _db.SaveChangesAsync();

			return new ResultWithMessage(null, string.Empty);
		}
	}
}