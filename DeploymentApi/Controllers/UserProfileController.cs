using AccountLib.Services.UserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeploymentApi.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserProfileController(IUserProfileService userProfileService) : BaseController
	{
		private readonly IUserProfileService _userProfileService = userProfileService;
		private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;


		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile() => HandleResult(await _userProfileService.GetProfileAsync(GetCurrentUserId()));


		[HttpGet("getUserRoles")]
		public async Task<IActionResult> GetUserRoles() => HandleResult(await _userProfileService.GetUserRolesAsync(GetCurrentUserId()));
	}
}