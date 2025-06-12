using AccountLib.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AccountLib.Models;
using AccountLib.Services.IdentityAccountService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AccountLib.Contracts;
using EmailSender.Services;
using AccountLib.Services.IdentityAccountService.JwtProvider;
using AccountLib.Services.IdentityAccountService.UsersService;
namespace AccountLib.Abstractions
{
	public static class IdentityServiceCollectionExtensions
	{
		public static IServiceCollection AddAccountIdentity(this IServiceCollection services, AccountIdentityParams accountIdentityParams)
		{
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(accountIdentityParams.ConfigurationManager.GetConnectionString(accountIdentityParams.ConnectionString)));

			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			// JWT Settings
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = accountIdentityParams.JwtSettings.Issuer,
					ValidAudience = accountIdentityParams.JwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accountIdentityParams.JwtSettings.Key))
				};
			});

			services.AddScoped<IIdentityAccountService, IdentityAccountService>();
			services.AddScoped<IJwtProvider, JwtProvider>();
			services.AddScoped<IUserService, UserService>();
			services.AddTransient<IEmailSenderService, EmailSenderService>();

			return services;
		}
	}
}