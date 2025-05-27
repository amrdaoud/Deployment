using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AccountLib.Entities
{
    public class ApplicationUserTenantRole : IdentityUserRole<Guid>
    {

        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }
        public virtual ApplicationTenant? Tenant { get; set; }
    }
}