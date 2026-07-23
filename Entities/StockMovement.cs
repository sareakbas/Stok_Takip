using System;

namespace Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public string MovementType { get; set; } = string.Empty; 
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; } 
        public decimal? TotalCost { get; set; } 
        public DateTime MovementDate { get; set; }

        // Foreign Keys
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? CustomerId { get; set; } // Çıkış kime yapıldı?
        public Customer? Customer { get; set; }

        public int? CreatedById { get; set; } // İşlemi kim yaptı?
        public User? CreatedBy { get; set; }
    }
}