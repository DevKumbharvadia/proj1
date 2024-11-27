using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Domain
{
    public class UserAudit
    {
        [Key]
        public Guid UserAuditId { get; set; } // Primary Key

        [Required]
        public Guid UserId { get; set; } // Foreign Key to User

        public DateTime LoginTime { get; set; } = DateTime.UtcNow; // Login timestamp

        public DateTime? LogoutTime { get; set; } // Optional logout timestamp

        public ICollection<UserActionLog> UserActionLogs { get; set; } = new List<UserActionLog>(); // Navigation property to UserActionLogs

        [Required]
        public User User { get; set; } = null!; // Navigation property to User
    }
}
