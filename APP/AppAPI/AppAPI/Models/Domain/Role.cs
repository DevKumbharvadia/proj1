using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.Domain
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
