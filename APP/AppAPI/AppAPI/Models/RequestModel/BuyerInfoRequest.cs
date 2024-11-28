using AppAPI.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.RequestModel
{
    public class BuyerInfoRequest
    {
        public Guid UserId { get; set; } // Foreign Key to User
        public string ContactNumber { get; set; } = null!;
        public string? Address { get; set; }
    }
}
