using Business.DTOs;
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
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.Where(s => s.IsActive).ToListAsync();
        }

        // Ekleme
        public async Task<(bool Success, string Message)> CreateSupplierAsync(CreateSupplierDto dto)
        {
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

            return (true, "Tedarikçi başarıyla eklendi.");
        }

        // Güncelleme
        public async Task<(bool Success, string Message)> UpdateSupplierAsync(UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(dto.Id);
            if (supplier == null)
            {
                return (false, "Güncellenecek tedarikçi bulunamadı.");
            }

            supplier.Name = dto.Name;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;

            await _context.SaveChangesAsync();
            return (true, "Tedarikçi başarıyla güncellendi.");
        }

        // Pasife Alma (Soft Delete)
        public async Task<(bool Success, string Message)> DeactivateSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return (false, "Pasife alınacak tedarikçi bulunamadı.");
            }

            supplier.IsActive = false;
            await _context.SaveChangesAsync();
            
            return (true, "Tedarikçi başarıyla pasife alındı.");
        }
    }
}