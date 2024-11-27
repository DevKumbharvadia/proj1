    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace AppAPI.Models.Domain
    {
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public byte[] ImageContent { get; set; } = Array.Empty<byte>();

        [Required]
        public double Price { get; set; }

        public int StockQuantity { get; set; } = 0;

        [Required]
        public Guid SellerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User Seller { get; set; }

        public ICollection<TransactionHistory> Transactions { get; set; } = new List<TransactionHistory>();
    }
}
