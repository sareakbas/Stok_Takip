namespace Business.Dtos
{
    public class CreateStockEntryDto
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}