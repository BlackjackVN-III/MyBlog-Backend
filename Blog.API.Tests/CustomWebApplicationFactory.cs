using Blog.Application.Interfaces;
using Blog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {

        public Mock<ICacheService> CacheServiceMock { get; }
       
        public CustomWebApplicationFactory()
        {
            CacheServiceMock = new Mock<ICacheService>();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Thiết lập các cấu hình giả lập (dummy settings) dành riêng cho môi trường Test
            // Việc này giúp tránh bị ghi đè lên các cấu hình SQL Server/Redis thật khi dev ở local
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=MyBlogTest;Trusted_Connection=True;TrustServerCertificate=True;" },
                    { "ConnectionStrings:RedisConnection", "localhost:6379" },
                    { "JWT:SigningKey", "SuperSecretKeyForMyBlogJWT_2026_MustBeAtLeast64CharactersLongForSecurity!!" },
                    { "JWT:Issuer", "BJVN_Blog" },
                    { "JWT:Audience", "BJVN_Blog_Client" },
                    { "JWT:TokenExpirationInMinutes", "60" },
                    { "JWT:RefreshTokenExpirationInDays", "7" },
                    { "CloudinarySettings:CloudName", "test-cloud" },
                    { "CloudinarySettings:ApiKey", "test-api-key" },
                    { "CloudinarySettings:ApiSecret", "test-api-secret" }
                });
            });

            builder.ConfigureServices(services =>
            {
                // 1. Gỡ bỏ đăng ký AppDbContext thật kết nối với SQL Server
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                // 2. Thay thế bằng EF Core In-Memory Database dành riêng cho test bằng cách dùng Service Provider riêng biệt để tránh xung đột với SQL Server
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting")
                           .UseInternalServiceProvider(serviceProvider);
                });
                // 3. Thay thế CacheService thật bằng đối tượng Mock để không phụ thuộc Redis
                var cacheDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICacheService));
                if (cacheDescriptor != null)
                {
                    services.Remove(cacheDescriptor);
                }
                services.AddScoped(sp => CacheServiceMock.Object);
            });
        }
    }
}
