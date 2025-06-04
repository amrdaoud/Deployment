using AccountLib.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentApi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class BaseController : ControllerBase
	{
		protected IActionResult HandleResult(ResultWithMessage result)
		{
			if (!string.IsNullOrEmpty(result.Message))
				return BadRequest(new { result.Message });

			return Ok(result);
		}
	}
}
