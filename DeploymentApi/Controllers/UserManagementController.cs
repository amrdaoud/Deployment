using AccountLib.Services.UserManagementService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentApi.Controllers
{
	[Authorize(Roles = "Admin, SuperUser")]
	[Route("api/[controller]")]
	[ApiController]
	public class UserManagementController(IUserManagementService userManagementService) : BaseController
	{
		private readonly IUserManagementService _userManagementService = userManagementService;

		[HttpGet("getAllUsers")]
		public async Task<IActionResult> GetAllUsers() => HandleResult(await _userManagementService.GetAllUsersAsync());
	}
}
