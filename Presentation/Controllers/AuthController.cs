using Business.Dtos;
using Business.Services;
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
            // İsteği AuthService'e gönderiyoruz
            var result = _authService.Register(dto);
            
            // Eğer dönen mesajda "Hata" veya "zaten kayıtlı" kelimeleri varsa, 400 Bad Request (Hatalı İstek) dönüyoruz.
            if (result.Contains("Hata") || result.Contains("zaten sistemde kayıtlı"))
                return BadRequest(result); 
                
            // İşlem başarılıysa 200 OK ile sonucu dönüyoruz.
            return Ok(result); 
        }

        // İstek Adresi: POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // İsteği AuthService'e gönderiyoruz
            var result = _authService.Login(dto);
            
            // Eğer şifre yanlış, hesap kilitli veya onaylanmamışsa hata mesajı döner
            if (result.Contains("Hata"))
                return BadRequest(result);
                
            // Başarılı giriş: Token'ı JSON formatında olarak front-end'e veriyoruz.
            return Ok(new { Token = result }); 
        }
    }
}