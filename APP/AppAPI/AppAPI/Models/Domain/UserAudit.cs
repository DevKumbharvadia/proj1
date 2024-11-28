using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class UserAudit
    {
        [Key]
        public Guid UserAuditId { get; set; } // Primary Key

        [Required]
        public Guid UserId { get; set; } // Foreign Key to User

        [Required]
        public DateTime LoginTime { get; set; } = DateTime.UtcNow; // Login timestamp

        public DateTime? LogoutTime { get; set; } // Optional logout timestamp

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!; // Navigation property to User
    }
}
