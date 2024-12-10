using AppAPI.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.RequestModel
{
    public class UserActionRequest
    {
        public Guid UserId { get; set; } // Foreign Key to UserAudit

        public string Action { get; set; } = null!; // Action name (e.g., "Create", "Delete")
    }
}
