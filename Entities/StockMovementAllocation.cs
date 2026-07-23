namespace Entities
{
    public class StockMovementAllocation
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        // Foreign Keys
        public int MovementId { get; set; }
        public StockMovement Movement { get; set; } = null!;

        public int LotId { get; set; }
        public StockLot Lot { get; set; } = null!;
    }
}