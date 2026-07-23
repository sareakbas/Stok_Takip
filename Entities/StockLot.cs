using System;

namespace Entities
{
    public class StockLot
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal UnitCost { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}