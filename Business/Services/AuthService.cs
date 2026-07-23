using BCrypt.Net;
using Business.Dtos;
using DataAccess;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services
{
    public class AuthService
    {
        private readonly StokTakipDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(StokTakipDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string Register(RegisterDto dto)
        {
           
            if (_context.Users.Any(u => u.Email == dto.Email))
            {
                return "Bu e-posta adresi zaten sistemde kayıtlı!";
            }

            // Yeni kullanıcıların her zaman personel rolüyle başlaması (K-03)
            var personelRole = _context.Roles.FirstOrDefault(r => r.Name == "Personel");
            int roleId = personelRole != null ? personelRole.Id : 2; // Eğer veritabanında henüz rol yoksa varsayılan olarak 2 veriyoruz.

            //  Şifreler BCrypt ile hashlenir (K-01)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Yeni kullanıcıyı oluşturuyoruz
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                RoleId = roleId,
                IsActive = false, // Admin onaylayana kadar pasif başlaması için (K-04)
                FailedLoginCount = 0
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return "Kayıt işlemi başarıyla tamamlandı! Yönetici onayından sonra giriş yapabilirsiniz.";
        }

        public string Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                return "Hata: Kullanıcı bulunamadı.";
            }

            // Kullanıcının hesabı 5 hatalı girişten dolayı kilitlenmiş mi
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.Now)
            {
                return $"Hata: Hesabınız kilitlendi. Lütfen {user.LockedUntil.Value:HH:mm} sonrasında tekrar deneyin.";
            }

            //  Kullanıcı aktif mi 
            if (!user.IsActive)
            {
                return "Hata: Hesabınız henüz onaylanmamış. Lütfen yöneticinizle iletişime geçin.";
            }

            
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            //şifre hatalıysa uygulanacak kurallar
            if (!isPasswordCorrect)
            {
                user.FailedLoginCount++;

                if (user.FailedLoginCount >= 5)
                {
                    user.LockedUntil = DateTime.Now.AddMinutes(15);
                    _context.SaveChanges();
                    return "Hata: Üst üste 5 kez hatalı giriş yaptığınız için hesabınız 15 dakika kilitlendi.";
                }

                _context.SaveChanges();
                return "Hata: Şifre yanlış.";
            }

            // şifre baaşrılıysa sayaçlar sıfırlanır
            user.FailedLoginCount = 0;
            user.LockedUntil = null;
            _context.SaveChanges();

            // giriş yapabilen kullanıcıya yeni token üretilir
            return GenerateJwtToken(user);
        }

        // Token Üretme Metodu 
        private string GenerateJwtToken(User user)
        {
            var roleName = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name ?? "Personel";

            // Uygulamamızın gizli anahtar kelimesi (appsettings.json'dan gelecek)
            var jwtKey = _configuration["Jwt:Key"] ?? "BenimCokGizliVeGuvenliAnahtarKelimen12345!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            // Token'ın kuralları (Örneğin 8 saat geçerli olacak)
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "StokTakipAPI",
                audience: _configuration["Jwt:Audience"] ?? "StokTakipKullanicilari",
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}