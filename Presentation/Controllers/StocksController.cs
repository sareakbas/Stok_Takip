using Business.Dtos;
using Business.Services;
using Business.Responses; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateStockEntry(CreateStockEntryDto dto)
        {
            var result = await _stockService.CreateStockEntryAsync(dto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}