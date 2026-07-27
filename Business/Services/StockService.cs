using Business.Dtos;
using Business.Responses; 
using DataAccess;
using Entities;

namespace Business.Services
{
    public class StockService
    {
        private readonly StokTakipDbContext _context;

        public StockService(StokTakipDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> CreateStockEntryAsync(CreateStockEntryDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || !product.IsActive)
            {
                return Result<bool>.ErrorResult(Messages.StockEntryProductNotFound);
            }

            var stockLot = new StockLot
            {
                ProductId = dto.ProductId,
                SupplierId = dto.SupplierId,
                EntryDate = DateTime.Now,
                InitialQuantity = dto.Quantity,
                RemainingQuantity = dto.Quantity,
                UnitCost = dto.UnitCost
            };

            _context.StockLots.Add(stockLot);

            var stockMovement = new StockMovement
            {
                ProductId = dto.ProductId,
                MovementType = "IN",
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitCost,
                MovementDate = DateTime.Now
            };

            _context.StockMovements.Add(stockMovement);

            await _context.SaveChangesAsync();

            // string.Format ile {0} yerine ürün adını, {1} yerine miktarı gönderiyoruz
            string successMessage = string.Format(Messages.StockEntrySuccessful, product.Name, dto.Quantity);
            
            return Result<bool>.SuccessResult(true, successMessage);
        }
    }
}