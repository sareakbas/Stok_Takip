namespace Business.Dtos
{
    public class StockOutDto
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; } 
        public decimal Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
    }
}