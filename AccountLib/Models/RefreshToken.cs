using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountLib.Models
{
	public class RefreshToken
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public string Token { get; set; } = string.Empty;

		public DateTime ExpiryDate { get; set; }
		public bool IsRevoked { get; set; } = false;


		public Guid UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser? User { get; set; }
	}
}