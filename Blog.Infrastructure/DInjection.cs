using Blog.Application.Interfaces;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Identity;
using Blog.Infrastructure.Repositories;
using Blog.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog.Infrastructure
{
    public static class DInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            // ===== 1. DATABASE =====
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký IAppDbContext để Handler có thể gọi SaveChangesAsync
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // ===== 2. IDENTITY =====
            // Cấu hình ASP.NET Identity: quản lý User, Role, Password rules
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // ===== 3. JWT AUTHENTICATION =====
            // Cấu hình middleware xác thực JWT Bearer
            // Khi client gửi request với Header "Authorization: Bearer eyJ...",
            // middleware sẽ tự động:
            //   1. Đọc token từ Header
            //   2. Giải mã token bằng SigningKey
            //   3. Kiểm tra Issuer, Audience, thời hạn
            //   4. Nếu hợp lệ → gắn thông tin user vào HttpContext.User
            //   5. Nếu không hợp lệ → trả về 401 Unauthorized
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Kiểm tra xem ai phát hành token (phải khớp với Issuer trong appsettings)
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],

                    // Kiểm tra token dành cho ai (phải khớp với Audience trong appsettings)
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],

                    // Kiểm tra chữ ký (quan trọng nhất - đảm bảo token không bị giả mạo)
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"]!)),

                    // Kiểm tra thời hạn token
                    ValidateLifetime = true,

                    // Không cho phép lệch giờ (token hết hạn là hết, không có thời gian gia hạn)
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ===== 4. DEPENDENCY INJECTION =====
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostRepository, PostRepository>();

            return services;
        }
    }
}
