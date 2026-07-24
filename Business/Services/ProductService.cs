using Business.DTOs;
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class ProductService
    {
        private readonly StokTakipDbContext _context;

        public ProductService (StokTakipDbContext context)
        {
            _context = context;
        }

    
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                         .Include(p => p.Category) 
                         .Where(p => p.IsActive)
                         .ToListAsync();
            

        }

        //  Ürün Ekleme ve Barkod Benzersizlik Kontrolü (K-07)
        public async Task<(bool Success, string Message)> CreateProductAsync(CreateProductDto dto)
        {
            
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == dto.Barcode);

            if (existingProduct != null)
            {
                return (false, "Bu barkoda sahip bir ürün zaten mevcut. Aynı barkodla ikinci bir ürün eklenemez.");
            }

           
            var product = new Product
            {
                Name = dto.Name,
                Barcode = dto.Barcode,
                Unit = dto.Unit,
                MinStockLevel = dto.MinStockLevel,
                CategoryId = dto.CategoryId,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return (true, "Ürün başarıyla eklendi.");
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null)
            {
                return (false, "Güncellenecek ürün bulunamadı.");
            }

            if (product.Barcode != dto.Barcode)
            {
                var barcodeExists = await _context.Products.AnyAsync(p => p.Barcode == dto.Barcode);
                if (barcodeExists)
                {
                    return (false, "Bu barkod başka bir ürün tarafından kullanılıyor.");
                }
            }

            product.Name = dto.Name;
            product.Barcode = dto.Barcode;
            product.Unit = dto.Unit;
            product.CategoryId = dto.CategoryId;
            product.MinStockLevel = dto.MinStockLevel;

            await _context.SaveChangesAsync();
            return (true, "Ürün başarıyla güncellendi.");
        }

        // Ürünü Pasife Alma Servisi (Soft Delete)
        public async Task<(bool Success, string Message)> DeactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return (false, "Pasife alınacak ürün bulunamadı.");
            }

            product.IsActive = false;
            await _context.SaveChangesAsync();
            
            return (true, "Ürün başarıyla pasife alındı ve listelerden kaldırıldı.");
        }

        // Ürünü Tekrar Aktifleştirme Servisi
        public async Task<(bool Success, string Message)> ReactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return (false, "Aktifleştirilecek ürün bulunamadı.");
            }

            if (product.IsActive)
            {
                return (false, "Bu ürün zaten aktif durumda.");
            }

            product.IsActive = true;
            await _context.SaveChangesAsync();

            return(true, "Ürün başarıyla tekrar aktifleştirildi ve lsitelere eklendi.");
    
        } 

    }

}