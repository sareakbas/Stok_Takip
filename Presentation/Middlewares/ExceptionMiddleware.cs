using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Business.Exceptions; 
using DataAccess; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (BusinessException ex)
            {
                // BİZİM FIRLATTIĞIMIZ ÖZEL HATA YAKALANDI
                await HandleBusinessExceptionAsync(context, ex);
            }
            catch (Exception)
            {
                // BEKLENMEDİK SİSTEMSEL BİR HATA 
                await HandleSystemExceptionAsync(context);
            }
        }

        private static async Task HandleBusinessExceptionAsync(HttpContext context, BusinessException ex)
        {
            // 1. Veritabanı bağlantımızı güvenli bir şekilde çağırıyoruz
            using var scope = context.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StokTakipDbContext>();

            // 2. Fırlatılan kodun ("ERR_STOCK_001") veritabanındaki karşılığını buluyoruz
            var errorRecord = await dbContext.ErrorMessages.FirstOrDefaultAsync(e => e.ErrorCode == ex.ErrorCode);
            
            string message = errorRecord != null ? errorRecord.MessageTr : "Bilinmeyen bir iş kuralı hatası oluştu.";

            if (ex.Parameters != null && ex.Parameters.Length > 0)
            {
                try
                {
                    message = string.Format(message, ex.Parameters);
                }
                catch (FormatException)
                {
                    // Eğer veritabanındaki süslü parantez sayısıyla gönderdiğimiz parametre sayısı uymazsa, 
                    // sistem çökmesin diye orijinal formatlanmamış mesajı bırakıyoruz.
                }
            }

            // 3. Müşteriye dönülecek HTTP 400 (Bad Request) JSON paketini hazırlıyoruz
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new 
            { 
                success = false, 
                message = message, 
                errorCode = ex.ErrorCode 
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        ////
        private static async Task HandleSystemExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new 
            { 
                success = false, 
                message = "Sistemsel bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." 
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}