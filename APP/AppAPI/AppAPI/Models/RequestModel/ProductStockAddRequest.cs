public class ProductStockAddRequest
{
    public Guid ProductId { get; set; }
    public int QuantityChanged { get; set; } // Positive for adding stock, negative for removing stock
}
