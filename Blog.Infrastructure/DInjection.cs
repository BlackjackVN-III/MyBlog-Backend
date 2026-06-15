using Blog.Application.Interfaces;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Identity;
using Blog.Infrastructure.Repositories;
using Blog.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Infrastructure
{
    public static class DInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
       

            services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); ;


            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPostRepository, PostRepository>();


            return services;
        }
    }
}
