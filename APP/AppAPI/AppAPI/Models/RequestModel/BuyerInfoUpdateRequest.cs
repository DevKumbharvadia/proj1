using AppAPI.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.RequestModel
{
    public class BuyerInfoUpdateRequest
    {
        public string ContactNumber { get; set; } = null!;
        public string? Address { get; set; }
    }
}
