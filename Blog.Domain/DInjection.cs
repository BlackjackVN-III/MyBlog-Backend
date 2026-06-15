using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain
{
    public static class DInjection 
    {
        public static  IServiceCollection AddDommainDI(this  IServiceCollection services) 
        {
            return services;
        }
    }
}
