using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Domain
{
    public class Action
    {
        [Key]
        public Guid ActionId { get; set; } // Primary Key

        [Required]
        public string ActionType { get; set; } = null!; // Action name (e.g., "Create", "Delete")

        public ICollection<UserActionLog> UserActionLogs { get; set; } = new List<UserActionLog>(); // Navigation property to UserActionLogs
    }
}
