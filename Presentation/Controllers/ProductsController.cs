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
    [Authorize] // Bu sınıftaki tüm işlemler için JWT Token (giriş yapmış olmak) zorunludur
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }
      

      //GET:api/products(Listeleme işlemi)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllProductsAsync();            
            return Ok(result);
        }

      //POST:api/products(Ekleme İşlemi)
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return Ok(result);
        }
     
      //PUT:güncelleme
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(dto); 
            return Ok(result);
        }


      //DELETE: pasife alma
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeactivateProductAsync(id);
            return Ok(result);
        }

      //PUT:Tekrar aktifleştirme
        [HttpPut("reactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var result = await _productService.ReactivateProductAsync(id);
            return Ok(result);
        }
    }
}