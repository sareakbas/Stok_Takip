using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Barkod zorunludur.")]
        [StringLength(50, ErrorMessage = "Barkod en fazla 50 karakter olabilir.")]
        public string Barcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birim (Adet, Kg vb.) zorunludur.")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Minimum stok seviyesi 0'dan küçük olamaz.")]
        public decimal MinStockLevel { get; set; }
    }
}