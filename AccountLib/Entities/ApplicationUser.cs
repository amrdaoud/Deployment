using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AccountLib.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<ApplicationUserTenantRole> ApplicationUserTenantRoles { get; set; } = new List<ApplicationUserTenantRole>();
    }
}