using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application
{
    public static class DInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
           
            return services;
        }
    }
}
