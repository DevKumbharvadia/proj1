using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class UserActionLog
    {
        [Key]
        public Guid ActionLogId { get; set; } // Primary Key

        [Required]
        public Guid UserAuditId { get; set; } // Foreign Key to UserAudit

        [Required]
        public Guid ActionId { get; set; } // Foreign Key to Action

        [Required]
        public DateTime ActionTimestamp { get; set; } = DateTime.UtcNow; // Timestamp when the action was performed

        [ForeignKey(nameof(ActionId))]
        public Action Action { get; set; } = null!; // Navigation property to Action

        [ForeignKey(nameof(UserAuditId))]
        public UserAudit UserAudit { get; set; } = null!; // Navigation property to UserAudit
    }
}
