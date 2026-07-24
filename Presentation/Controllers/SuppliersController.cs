using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class SuppliersController : ControllerBase
    {
        private readonly SupplierService _supplierService;

        public SuppliersController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            var result = await _supplierService.CreateSupplierAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateSupplierDto dto)
        {
            var result = await _supplierService.UpdateSupplierAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _supplierService.DeactivateSupplierAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}