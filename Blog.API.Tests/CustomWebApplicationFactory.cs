using Blog.Application.Interfaces;
using Blog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
