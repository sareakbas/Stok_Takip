using Entities;
using Business.Dtos;
using DataAccess;
using System;
using System.Linq; 
using System.Threading.Tasks;
using Business.Responses;
using Business.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class StockService
    {
        private readonly StokTakipDbContext _context;

        public StockService(StokTakipDbContext context)
        {
            _context = context;
        }

        // Stok Girişi (Alış)
        public async Task<Result<bool>> CreateStockEntryAsync(StockEntryDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || !product.IsActive)
            {
                throw new BusinessException("ERR_STK_001");
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

               var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_STK_001");
               string template = successRecord?.MessageTr ?? "";
               string successMessage = string.IsNullOrEmpty(template) ? "" : string.Format(template, product.Name, dto.Quantity);
           
               return Result<bool>.SuccessResult(true, successMessage);
            }
           

        // Stok Çıkışı (Satış) ve FIFO Algoritması
        public async Task<Result<bool>> CreateStockOutAsync(StockOutDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

        
            // 1. Ürünü Kontrol Et
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || !product.IsActive)
            {
                throw new BusinessException("ERR_STK_002");
            }

            // 2. Toplam Stok Yeterliliğini Kontrol Et
            var totalAvailableStock = _context.StockLots
                    .Where(l => l.ProductId == dto.ProductId && l.RemainingQuantity > 0)
                    .Sum(l => l.RemainingQuantity);

                if (totalAvailableStock < dto.Quantity)
                {
                    throw new BusinessException("ERR_STK_004", totalAvailableStock, dto.Quantity);
                }

            // 3. Stok Çıkış (OUT) Hareketini ÖNCE Kaydet (MovementId'yi alabilmek için)
            var stockMovement = new StockMovement
                {
                    ProductId = dto.ProductId,
                    CustomerId = dto.CustomerId, 
                    MovementType = "OUT", 
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice, 
                    MovementDate = DateTime.Now,
                    CreatedById = userId
                };

                await _context.StockMovements.AddAsync(stockMovement);
                await _context.SaveChangesAsync(); // Bu satır çalıştığında SQL, stockMovement.Id değerini üretir.

            var availableLots = _context.StockLots
                    .Where(l => l.ProductId == dto.ProductId && l.RemainingQuantity > 0)
                    .OrderBy(l => l.EntryDate) 
                    .ToList();

            decimal remainingQuantityToDeduct = dto.Quantity; 

                
                foreach (var lot in availableLots)
                {
                    if (remainingQuantityToDeduct <= 0) 
                        break; // Düşülecek miktar kalmadıysa döngüden çık

                    // Bu partiden ne kadar düşeceğimizi tutacak geçici değişken
                    decimal deductedAmount = 0;

                    if (lot.RemainingQuantity >= remainingQuantityToDeduct)
                    {
                        // Bu partinin stoğu, ihtiyacı karşılıyor
                        deductedAmount = remainingQuantityToDeduct;
                        lot.RemainingQuantity -= remainingQuantityToDeduct;
                        remainingQuantityToDeduct = 0;
                    }
                    else
                    {
                        // Bu partinin stoğu yetmiyor, içindeki her şeyi alıp partiyi sıfırlıyoruz
                        deductedAmount = lot.RemainingQuantity;
                        remainingQuantityToDeduct -= lot.RemainingQuantity;
                        lot.RemainingQuantity = 0; 
                    }

                    _context.StockLots.Update(lot);

                    // 5. ALLOCATION (Dağıtım) Kaydını Oluştur
                    var allocation = new StockMovementAllocation
                    {
                        MovementId = stockMovement.Id, 
                        LotId = lot.Id,                
                        Quantity = deductedAmount,    
                        UnitCost = lot.UnitCost       
                    };

                    await _context.StockMovementAllocations.AddAsync(allocation);
                }

                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

               var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_STK_002");
               string template = successRecord?.MessageTr ?? "";
               string successMessage = string.IsNullOrEmpty(template) ? "" : string.Format(template, product.Name, dto.Quantity);
            
               return Result<bool>.SuccessResult(true, successMessage);
        }
    }
}