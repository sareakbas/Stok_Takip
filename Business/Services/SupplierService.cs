using Business.DTOs;
using Business.Responses; 
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;
using Business.Exceptions;

namespace Business.Services
{
    public class SupplierService
    {
        private readonly StokTakipDbContext _context;

        public SupplierService(StokTakipDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<Result<List<Supplier>>> GetAllSuppliersAsync()
        {
            var suppliers = await _context.Suppliers.Where(s => s.IsActive).ToListAsync();
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_SUP_001");
            return Result<List<Supplier>>.SuccessResult(suppliers, successRecord?.MessageTr ?? "");
        }

        // Ekleme
        public async Task<Result<bool>> CreateSupplierAsync(CreateSupplierDto dto)
        {
            bool supplierExists = await _context.Suppliers
                .AnyAsync(s => s.Email == dto.Email || s.Phone == dto.Phone);

            if (supplierExists)
            {
               throw new BusinessException("ERR_SUP_001");
            }

            var supplier = new Supplier
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_SUP_002");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        // Güncelleme
        public async Task<Result<bool>> UpdateSupplierAsync(UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(dto.Id);
            if (supplier == null)
            {
                throw new BusinessException("ERR_SUP_002");
            }

            supplier.Name = dto.Name;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;

            await _context.SaveChangesAsync();
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_SUP_003");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        // Pasife Alma (Soft Delete)
        public async Task<Result<bool>> DeactivateSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
               throw new BusinessException("ERR_SUP_002");
            }

            supplier.IsActive = false;
            await _context.SaveChangesAsync();
            
           var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_SUP_004");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }
    }
}