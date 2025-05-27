using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AccountLib.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public virtual ApplicationUser? User { get; set; }
    }
}