using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Güncellenecek ürünün ID'si zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Barkod zorunludur.")]
        public string Barcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birim (Adet, Kg vb.) zorunludur.")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        public decimal MinStockLevel { get; set; }
    }
}