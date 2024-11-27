using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class ProductStockLog
    {
        [Key]
        public Guid StockLogId { get; set; } // Primary Key

        [Required]
        public Guid ProductId { get; set; } // Foreign Key to Product

        [Required]
        public int QuantityChanged { get; set; }

        public int NewStockLevel { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // When the change occurred
        public Product Product { get; set; } = null!; // Navigation property to Product
    }
}
