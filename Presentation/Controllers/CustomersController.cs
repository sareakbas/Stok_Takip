using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Hem Admin hem Personel erişebilir
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        // 1. Müşterileri Listeleme (GET: api/customers)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // 2. Yeni Müşteri Ekleme (POST: api/customers)
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            var result = await _customerService.CreateCustomerAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        // 3. Müşteri Güncelleme (PUT: api/customers)
        [HttpPut]
        public async Task<IActionResult> Update(UpdateCustomerDto dto)
        {
            var result = await _customerService.UpdateCustomerAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        // 4. Müşteriyi Pasife Alma (DELETE: api/customers/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _customerService.DeactivateCustomerAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}