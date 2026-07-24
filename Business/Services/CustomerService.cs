using Business.DTOs;
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class CustomerService
    {
        private readonly StokTakipDbContext _context;

        public CustomerService(StokTakipDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.Where(c => c.IsActive).ToListAsync();
        }

        // Ekleme
        public async Task<(bool Success, string Message)> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return (true, "Müşteri başarıyla eklendi.");
        }

        // Güncelleme
        public async Task<(bool Success, string Message)> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.Id);
            if (customer == null)
            {
                return (false, "Güncellenecek müşteri bulunamadı.");
            }

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();
            return (true, "Müşteri başarıyla güncellendi.");
        }

        // Pasife Alma (Soft Delete K-18)
        public async Task<(bool Success, string Message)> DeactivateCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return (false, "Pasife alınacak müşteri bulunamadı.");
            }

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            
            return (true, "Müşteri başarıyla pasife alındı.");
        }
    }
}