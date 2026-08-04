using BCrypt.Net;
using Business.Dtos;
using DataAccess;
using Business.Responses;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Exceptions;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<bool>> Register(RegisterDto dto)
        {
           
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
              throw new BusinessException("ERR_ATH_001");
            }

            // Yeni kullanıcıların her zaman personel rolüyle başlaması (K-03)
            var personelRole = _context.Roles.FirstOrDefault(r => r.Name == "Personel");
            int roleId = personelRole != null ? personelRole.Id : 2; // Eğer veritabanında henüz rol yoksa varsayılan olarak 2 veriyoruz.

            //  Şifreler BCrypt ile hashlenir (K-01)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            
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
            await _context.SaveChangesAsync();

            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_ATH_001");
            return Result<bool>.SuccessResult(true, successRecord?.MessageTr ?? "");
        }

        public async Task<Result<string>> Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                throw new BusinessException("ERR_ATH_002");
            }

            // Kullanıcının hesabı 5 hatalı girişten dolayı kilitlenmiş mi
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.Now)
            {
                string timeString = user.LockedUntil.Value.ToString("HH:mm");
               throw new BusinessException("ERR_ATH_003", timeString);
            }

           
            if (!user.IsActive)
            {
               throw new BusinessException("ERR_ATH_004");
            }

            
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            
            if (!isPasswordCorrect)
            {
                user.FailedLoginCount++;

                if (user.FailedLoginCount >= 5)
                {
                    user.LockedUntil = DateTime.Now.AddMinutes(15);
                    await _context.SaveChangesAsync();
                    throw new BusinessException("ERR_ATH_005");
                }

               await _context.SaveChangesAsync();
               throw new BusinessException("ERR_ATH_006");
            }

            // şifre baaşrılıysa sayaçlar sıfırlanır
            user.FailedLoginCount = 0;
            user.LockedUntil = null;
            await _context.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            // Başarılıysa Data olarak token'ı veriyoruz
            var successRecord = await _context.ErrorMessages.FirstOrDefaultAsync(m => m.ErrorCode == "SUC_ATH_002");
            return Result<string>.SuccessResult(token, successRecord?.MessageTr ?? "");
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