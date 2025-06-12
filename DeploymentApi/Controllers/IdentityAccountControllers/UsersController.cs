using AccountLib.Services.IdentityAccountService.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeploymentApi.Controllers.IdentityAccountControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(IUserService userService) : BaseController
	{
		private readonly IUserService _userService = userService;


		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			var result = await _userService.GetProfileAsync(userId!);
			return HandleResult(result);
		}
	}
}