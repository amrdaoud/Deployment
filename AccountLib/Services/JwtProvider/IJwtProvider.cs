﻿using AccountLib.Models;

namespace AccountLib.Services.IdentityAccountService.JwtProvider
{
	public interface IJwtProvider
	{
		Task<(string Token, DateTime ExpiryDate)> GenerateJwtTokenAsync(ApplicationUser user);
		(string Token, DateTime ExpiryDate) GenerateSecureToken();
	}
}
