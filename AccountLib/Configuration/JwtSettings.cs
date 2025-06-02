﻿namespace AccountLib.Configuration
{
	public class JwtSettings
	{
		public string Issuer { get; set; } = default!;
		public string Audience { get; set; } = default!;
		public string Key { get; set; } = default!;
		public int DurationInMinutes { get; set; }
	}
}
