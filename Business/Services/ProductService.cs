using Business.DTOs;
using Business.Responses; 
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;
using Business.Exceptions;

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
            
           var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_PRD_001");
            return Result<List<Product>>.SuccessResult(products, successRecord?.MessageTr ?? "");
        }

        public async Task<Result<bool>> CreateProductAsync(CreateProductDto dto)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == dto.Barcode);

            if (existingProduct != null)
            {
                throw new BusinessException("ERR_PRD_001");
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

           var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_PRD_002");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        public async Task<Result<bool>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null)
            {
                throw new BusinessException("ERR_PRD_002");
            }

            if (product.Barcode != dto.Barcode)
            {
                var barcodeExists = await _context.Products.AnyAsync(p => p.Barcode == dto.Barcode);
                if (barcodeExists)
                {
                    throw new BusinessException("ERR_PRD_003");
                }
            }

            product.Name = dto.Name;
            product.Barcode = dto.Barcode;
            product.Unit = dto.Unit;
            product.CategoryId = dto.CategoryId;
            product.MinStockLevel = dto.MinStockLevel;

            await _context.SaveChangesAsync();
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_PRD_003");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        public async Task<Result<bool>> DeactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new BusinessException("ERR_PRD_002");
            }

            product.IsActive = false;
            await _context.SaveChangesAsync();
            
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_PRD_004");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        public async Task<Result<bool>> ReactivateProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new BusinessException("ERR_PRD_002");
            }

            if (product.IsActive)
            {
                throw new BusinessException("ERR_PRD_004");
            }

            product.IsActive = true;
            await _context.SaveChangesAsync();

            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_PRD_005");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        } 
    }
}