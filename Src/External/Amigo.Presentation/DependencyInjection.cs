using Amigo.Application;
using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Services;
using Amigo.Presentation.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ResultFilter>();
            services.AddControllers(options =>
            {

                options.Filters.Add<ResultFilter>();
            });
            return services;    
        }
    }
}
