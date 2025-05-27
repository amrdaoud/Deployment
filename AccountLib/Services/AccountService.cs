using System.Text;
using AccountLib.Data;
using AccountLib.DTOs;
using AccountLib.Entities;
using AccountLib.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AccountLib.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        public AccountService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext db)
        {
            _userManager = userManager;
            _config = config;
            _db = db;
        }

        // private async Task<AuthenticationModel> GenerateTokenAsync(ApplicationUser user)
        // {
        //     var jwtSettings = _config.GetSection("JwtSettings");
        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
        //     var tenantRoles = _db.ApplicationUserTenantRoles.Where(x => x.UserId == user.Id).Select(y => new
        //     {
        //         TenantName = y.Tenant.Name,
        //         Role = _db.Roles.FirstOrDefault(r => r.Id == y.RoleId).Name
        //     });
        // }
    }
}