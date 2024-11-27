using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class BuyerInfo
    {
        [Key]
        public Guid BuyerInfoId { get; set; } // Primary Key

        [Required]
        public Guid UserId { get; set; } // Foreign Key to User

        [Required]
        [Phone]
        public string ContactNumber { get; set; } = null!;

        public string? Address { get; set; }

        public User User { get; set; } = null!; // Navigation property to User
    }
}
