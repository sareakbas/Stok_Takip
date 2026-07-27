using Business.Services; 
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// JWT Doğrulama Ayarları
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "StokTakipAPI", 
            ValidAudience = "StokTakipKullanicilari", 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BenimCokGizliVeGuvenliAnahtarKelimen12345!")) 
        };

        options.Events = new JwtBearerEvents
        {
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = "{\"message\": \"Bu eylemi gerçekleştirmek için yetkiniz (Admin) bulunmamaktadır.\"}";
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddOpenApi();

// Veritabanı bağlantı servisini projeye tanıtıyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StokTakipDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<StockService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Kimlik doğrulama
app.UseAuthorization();  // Yetki doğrulama


app.MapControllers();

app.Run();