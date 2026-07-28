using Entities;
using Business.Dtos;
using DataAccess;
using System;
using System.Threading.Tasks;
using Business.Responses;

namespace Business.Services
{
    public class StockService
    {
        private readonly StokTakipDbContext _context;

        public StockService(StokTakipDbContext context)
        {
            _context = context;
        }

      public async Task<Result<bool>> CreateStockEntryAsync(StockEntryDto dto, int userId)
        {
            
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null || !product.IsActive)
                {
                    return Result<bool>.ErrorResult(Messages.StockEntryProductNotFound);
                }


                // Yeni Parti (Lot) Oluşturma (K-10 Kuralı)
                var newLot = new StockLot
                {
                    ProductId = dto.ProductId,
                    SupplierId = dto.SupplierId,
                    EntryDate = DateTime.Now,
                    InitialQuantity = dto.Quantity,
                    RemainingQuantity = dto.Quantity, 
                    UnitCost = dto.UnitCost
                };
                
                await _context.StockLots.AddAsync(newLot);

                // Stok Hareketi (Log/Geçmiş) Oluşturma
                var stockMovement = new StockMovement
                {
                    ProductId = dto.ProductId,
                    MovementType = "IN", 
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitCost,
                    MovementDate = DateTime.Now,
                    CreatedById = userId
                };

                await _context.StockMovements.AddAsync(stockMovement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                string successMessage = string.Format(Messages.StockEntrySuccessful, product.Name, dto.Quantity);
                return Result<bool>.SuccessResult(true, successMessage);
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                string errorMessage = string.Format(Messages.StockEntryFailed, ex.Message);
                return Result<bool>.ErrorResult(errorMessage);
            }
        }
    }
}