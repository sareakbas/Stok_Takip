using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bu sınıftaki tüm işlemler için JWT Token (giriş yapmış olmak) zorunludur
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products (Listeleme işlemi - Tüm giriş yapanlar görebilir)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // POST: api/products (Ekleme işlemi - SADECE ADMIN yapabilir)
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        // 3. GÜNCELLEME (PUT) - Sadece Admin yapabilir
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        // 4. PASİFE ALMA / SİLME (DELETE) - Sadece Admin yapabilir
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeactivateProductAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        // 5. TEKRAR AKTİFLEŞTİRME (PUT) - Sadece Admin yapabilir
        [HttpPut("reactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var result = await _productService.ReactivateProductAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

    }
}