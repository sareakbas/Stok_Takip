using Business.Dtos;
using Business.Services;
using Business.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // İstek Adresi: POST api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var result = _authService.Register(dto);
            
            if (result.Success)
            {
                return Ok(result); 
            }

            return BadRequest(result);
        }

        // İstek Adresi: POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            
            var result = _authService.Login(dto);
            
            if (result.Success)
            {
                return Ok(result);
            }
                
            return BadRequest(result);
        }
    }
}