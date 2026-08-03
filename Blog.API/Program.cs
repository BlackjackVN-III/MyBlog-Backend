
using Blog.API.OpenAPI;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Hubs;
using Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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

            builder.Services.AddAppDI(builder.Configuration);

            builder.Services.AddSignalR();


            var app = builder.Build();

            // ===== RUN DATABASE MIGRATIONS =====
            // Tự động chạy Migration để tạo bảng/cấu trúc DB nếu chưa có (rất quan trọng khi deploy lên Azure SQL trống)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
                {
                    await dbContext.Database.MigrateAsync();
                }
            }

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

            // THỨ TỰ RẤT QUAN TRỌNG:
            // 1. UseAuthentication() - Đọc JWT từ Header, xác thực, gắn User vào HttpContext
            // 2. UseAuthorization()  - Kiểm tra xem User đã xác thực có quyền truy cập endpoint không
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/hubs/notification");

            app.MapControllers();

            app.Run();
        }
    }
}
