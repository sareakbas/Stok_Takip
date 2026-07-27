using Business.DTOs;
using Business.Responses; 
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;

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
            return Result<List<Supplier>>.SuccessResult(suppliers, Messages.SupplierListed);
        }

        // Ekleme
        public async Task<Result<bool>> CreateSupplierAsync(CreateSupplierDto dto)
        {
            bool supplierExists = await _context.Suppliers
                .AnyAsync(s => s.Email == dto.Email || s.Phone == dto.Phone);

            if (supplierExists)
            {
                return Result<bool>.ErrorResult(Messages.SupplierAlreadyExists);
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

            return Result<bool>.SuccessResult(true, Messages.SupplierAdded);
        }

        // Güncelleme
        public async Task<Result<bool>> UpdateSupplierAsync(UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(dto.Id);
            if (supplier == null)
            {
                return Result<bool>.ErrorResult(Messages.SupplierNotFound);
            }

            supplier.Name = dto.Name;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;

            await _context.SaveChangesAsync();
            return Result<bool>.SuccessResult(true, Messages.SupplierUpdated);
        }

        // Pasife Alma (Soft Delete)
        public async Task<Result<bool>> DeactivateSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return Result<bool>.ErrorResult(Messages.SupplierNotFound);
            }

            supplier.IsActive = false;
            await _context.SaveChangesAsync();
            
            return Result<bool>.SuccessResult(true, Messages.SupplierDeleted);
        }
    }
}