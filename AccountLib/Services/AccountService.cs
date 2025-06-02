//using AccountLib.Data;
//using AccountLib.DTOs.Request;
//using AccountLib.Entities;
//using AccountLib.Services.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;

//namespace AccountLib.Services
//{
//	public class AccountService(
//		ApplicationDbContext db,
//		UserManager<ApplicationUser> userManager,
//		RoleManager<ApplicationRole> roleManager,
//		IConfiguration config) : IAccountService
//	{
//		private readonly UserManager<ApplicationUser> _userManager = userManager;
//		private readonly IConfiguration _config = config;
//		private readonly ApplicationDbContext _db = db;
//		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

//		public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
//		{
//			var user = new ApplicationUser
//			{
//				UserName = request.UserName,
//				Email = request.Email,
//				EmailConfirmed = true,
//				NormalizedEmail = request.Email.ToUpper(),
//				NormalizedUserName = request.UserName.ToUpper()
//			};

//			var result = await _userManager.CreateAsync(user, request.Password);

//			if (!result.Succeeded)
//				return result;

//			foreach (var tenantRole in request.TenantRoles)
//			{
//				foreach (var roleId in tenantRole.RoleIds)
//				{
//					var role = await _roleManager.FindByIdAsync(roleId);

//					if (role == null)
//						continue;

//					_db.UserRoles.Add(new ApplicationUserTenantRole
//					{
//						UserId = user.Id,
//						RoleId = Guid.Parse(roleId),
//						TenantId = Guid.Parse(tenantRole.TenantId),
//					});
//				}
//			}

//			await _db.SaveChangesAsync();
//			return IdentityResult.Success;
//		}
//	}
//}