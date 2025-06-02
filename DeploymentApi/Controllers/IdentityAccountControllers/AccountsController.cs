using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Services.IdentityAccountService;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentApi.Controllers.IdentityAccountControllers
{
	[Route("api/identityAccount/[controller]")]
	[ApiController]
	public class AccountsController(IIdentityAccountService IdentityAccountService) : ControllerBase
	{
		private readonly IIdentityAccountService _IdentityAccountService = IdentityAccountService;

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var result = await _IdentityAccountService.RegisterAsync(request);

			if (!string.IsNullOrEmpty(result.Message))
				return BadRequest(new { result.Message });

			return Ok();
		}
	}
}
