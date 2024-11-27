using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Domain
{
    public class UserAction
    {
        [Key]
        public Guid UserActionId { get; set; } // Primary Key

        [Required]
        public Guid ActionLogId { get; set; } // Foreign Key to UserActionLog

        [Required]
        public Guid ActionId { get; set; } // Foreign Key to Action

        [Required]
        public UserActionLog UserActionLog { get; set; } = null!; // Navigation property to UserActionLog

        [Required]
        public Action Action { get; set; } = null!; // Navigation property to Action
    }
}
