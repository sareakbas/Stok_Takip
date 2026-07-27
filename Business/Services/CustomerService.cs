using Business.DTOs;
using Business.Responses;
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
        public async Task<Result<List<Customer>>> GetAllCustomersAsync()
        {
           var customers = await _context.Customers.Where(c => c.IsActive).ToListAsync();
            
            return Result<List<Customer>>.SuccessResult(customers, Messages.CustomerListed);
        }

        // Ekleme
        public async Task<Result<bool>> CreateCustomerAsync(CreateCustomerDto dto)
        {

            bool customerExists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email || c.Phone == dto.Phone);

            if (customerExists)
            {
                return Result<bool>.ErrorResult(Messages.CustomerAlreadyExists);
            }

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

            return Result<bool>.SuccessResult(true, Messages.CustomerAdded);
        }

        // Güncelleme
       public async Task<Result<bool>> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.Id);
            if (customer == null)
            {
               return Result<bool>.ErrorResult(Messages.CustomerNotFound);
            }

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();
            return Result<bool>.SuccessResult(true, Messages.CustomerUpdated);
        }

        // Pasife Alma (Soft Delete K-18)
        public async Task<Result<bool>> DeactivateCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return Result<bool>.ErrorResult(Messages.CustomerNotFound);
            }

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            
            return Result<bool>.SuccessResult(true, Messages.CustomerDeleted);
        }
    }
}