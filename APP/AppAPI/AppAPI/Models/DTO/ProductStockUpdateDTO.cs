public class ProductStockUpdateDTO
{
    public Guid ProductId { get; set; }
    public int QuantityChanged { get; set; } // Positive for adding stock, negative for removing stock
    public string ChangeReason { get; set; } = string.Empty; // Reason for the change (e.g., "restock", "sale", etc.)
}
