namespace TodoAPI.Models
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty; // Initialize to avoid null reference issues

        public bool Success { get; set; }

        public T? Data { get; set; }  // Nullable reference type for T

    }
}
