using Business.DTOs;
using Business.Responses; 
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

        public async Task<Result<List<Product>>> GetAllProductsAsync()
        {
            var products = await _context.Products
                         .Include(p => p.Category) 
                         .Where(p => p.IsActive)
                         .ToListAsync();
            
            return Result<List<Product>>.SuccessResult(products, Messages.ProductListed);
        }

        public async Task<Result<bool>> CreateProductAsync(CreateProductDto dto)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == dto.Barcode);

            if (existingProduct != null)
            {
                return Result<bool>.ErrorResult(Messages.ProductBarcodeAlreadyExists);
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

            return Result<bool>.SuccessResult(true, Messages.ProductAdded);
        }

        public async Task<Result<bool>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null)
            {
                return Result<bool>.ErrorResult(Messages.ProductNotFound);
            }

            if (product.Barcode != dto.Barcode)
            {
                var barcodeExists = await _context.Products.AnyAsync(p => p.Barcode == dto.Barcode);
                if (barcodeExists)
                {
                    return Result<bool>.ErrorResult(Messages.ProductBarcodeUsedByAnother);
                }
            }

            product.Name = dto.Name;
            product.Barcode = dto.Barcode;
            product.Unit = dto.Unit;
            product.CategoryId = dto.CategoryId;
            product.MinStockLevel = dto.MinStockLevel;

            await _context.SaveChangesAsync();
            return Result<bool>.SuccessResult(true, Messages.ProductUpdated);
        }

        public async Task<Result<bool>> DeactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Result<bool>.ErrorResult(Messages.ProductNotFound);
            }

            product.IsActive = false;
            await _context.SaveChangesAsync();
            
            return Result<bool>.SuccessResult(true, Messages.ProductDeactivated);
        }

        public async Task<Result<bool>> ReactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Result<bool>.ErrorResult(Messages.ProductNotFound);
            }

            if (product.IsActive)
            {
                return Result<bool>.ErrorResult(Messages.ProductAlreadyActive);
            }

            product.IsActive = true;
            await _context.SaveChangesAsync();

            return Result<bool>.SuccessResult(true, Messages.ProductReactivated);
        } 
    }
}