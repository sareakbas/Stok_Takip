using Business.DTOs;
using Business.Responses;
using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;
using Business.Exceptions;

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
            
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_CUS_001");
            return Result<List<Customer>>.SuccessResult(customers, successRecord?.MessageTr ?? "");
        }

        // Ekleme
        public async Task<Result<bool>> CreateCustomerAsync(CreateCustomerDto dto)
        {

            bool customerExists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email || c.Phone == dto.Phone);

            if (customerExists)
            {
                throw new BusinessException("ERR_CUS_001");
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

            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_CUS_002");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        // Güncelleme
       public async Task<Result<bool>> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.Id);
            if (customer == null)
            {
               throw new BusinessException("ERR_CUS_002");
            }

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_CUS_003");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        // Pasife Alma (Soft Delete K-18)
        public async Task<Result<bool>> DeactivateCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
               throw new BusinessException("ERR_CUS_002");
            }

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_CUS_004");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }
    }
}