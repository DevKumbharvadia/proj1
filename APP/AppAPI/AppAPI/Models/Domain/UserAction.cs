using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class UserAction
    {
        [Key]
        public Guid UserActionId { get; set; } // Primary Key

        [Required]
        public Guid UserAuditId { get; set; } // Foreign Key to UserAudit

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = null!; // Action name (e.g., "Create", "Delete")

        [Required]
        public DateTime TimeOfAction { get; set; } = DateTime.UtcNow; // UTC timestamp of the action

        [ForeignKey(nameof(UserAuditId))]
        public UserAudit UserAudit { get; set; } = null!; // Navigation property to UserAudit
    }
}
