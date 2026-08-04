using Business.Dtos;
using Business.Services;
using Business.Responses; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class StocksController : ControllerBase
    {
        private readonly StockService _stockService;

        public StocksController(StockService stockService)
        {
            _stockService = stockService;
        }

        // Stok Girişi (POST: api/stocks/entry)
        [HttpPost("entry")]
        public async Task<IActionResult> CreateStockEntry(StockEntryDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var result = await _stockService.CreateStockEntryAsync(dto, userId);
            
             return Ok(result);
        }

        // Stok Çıkışı (POST: api/stocks/out)
        [HttpPost("out")]
        public async Task<IActionResult> CreateStockOut([FromBody] StockOutDto dto)
        {
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }


            var result = await _stockService.CreateStockOutAsync(dto, userId);
            
            return Ok(result);
        }
    }
}