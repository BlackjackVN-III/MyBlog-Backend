
using Blog.API.OpenAPI;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Hubs;
using Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

namespace Blog.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            // ===== CORS =====
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    var allowedOrigins = builder.Configuration
                        .GetSection("AllowedOrigins")
                        .Get<string[]>() ?? Array.Empty<string>();

                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // ===== RATE LIMITING =====
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var retryAfter = 60;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retrySpan))
                    {
                        retryAfter = Math.Max(1, (int)retrySpan.TotalSeconds);
                    }
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

                    var responseObj = new
                    {
                        status = 429,
                        message = "Bạn đang gửi yêu cầu quá nhanh. Vui lòng thử lại sau ít phút.",
                        retryAfterSeconds = retryAfter
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(responseObj, cancellationToken);
                };

                // 1. General policy: 60 requests / minute / IP
                options.AddPolicy("general-policy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: GetClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // 2. Auth policy: 5 requests / minute / IP (chống brute-force đăng nhập / đăng ký)
                options.AddPolicy("auth-policy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: GetClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // 3. Interactive policy: 15 requests / minute / IP (cho comment, tạo bài, upload)
                options.AddPolicy("interactive-policy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: GetClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 15,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));
            });

            builder.Services.AddAppDI(builder.Configuration);

            builder.Services.AddSignalR();


            var app = builder.Build();

            // ===== SEED ROLES =====
            // Tạo sẵn các Role mặc định trong DB khi ứng dụng khởi động.
            // Chỉ tạo nếu chưa tồn tại, không ảnh hưởng nếu đã có.
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseRateLimiter();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/hubs/notification").RequireRateLimiting("general-policy");

            app.MapControllers();

            app.Run();
        }

        private static string GetClientIp(HttpContext httpContext)
        {
            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedHeader))
            {
                var ip = forwardedHeader.Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    return ip;
                }
            }
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        }
    }
}
