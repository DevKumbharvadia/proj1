using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPI.Models.Domain
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; } // Primary Key

        [Required]
        public Guid ProductId { get; set; } // Foreign Key to Product

        [Required]
        public Guid BuyerId { get; set; } // Foreign Key to User (Buyer)

        [Required]
        public Guid SellerId { get; set; } // Foreign Key to User (Seller)

        [Required]
        public int Quantity { get; set; } // Not Null

        [Required]
        public double TotalAmount { get; set; } // Not Null

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow; // Default value

        [Required]
        public bool OrderStatus { get; set; } = false; // Default order status

        public Product Product { get; set; } = null!; // Ensures non-null Product relationship

        public User Buyer { get; set; } = null!; // Buyer navigation property

        public User Seller { get; set; } = null!; // Seller navigation property
    }
}
