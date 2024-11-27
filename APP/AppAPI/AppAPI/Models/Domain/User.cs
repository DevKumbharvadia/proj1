using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Domain
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<TransactionHistory> Transactions { get; set; } = new List<TransactionHistory>();

        public ICollection<UserAudit> UserAudits { get; set; } = new List<UserAudit>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
