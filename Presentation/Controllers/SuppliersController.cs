using Business.DTOs;
using Business.Services;
using Business.Responses; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            var result = await _supplierService.GetAllSuppliersAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            var result = await _supplierService.CreateSupplierAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateSupplierDto dto)
        {
            var result = await _supplierService.UpdateSupplierAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _supplierService.DeactivateSupplierAsync(id);
            return Ok(result);
        }
    }
}