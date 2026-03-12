
using Amigo.API.MiddleWareExceptions;
using Amigo.Application;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Entities;
using Amigo.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Amigo.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddBasicDependencyInjcetion(builder.Configuration)
                            .AddPersistence(builder.Configuration)
                            .AddApplicationDependencyInjection();
            

            builder.Services.AddScoped< IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();



            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            #region AllowCors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });

            });

            #endregion

            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            #region SeedingData

            using var scope = app.Services.CreateScope();
            var seedObj =  scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await seedObj.IdentityDataSeedAsync();
            



            #endregion

            app.UseMiddleware<MiddleWareCustomException>();

            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseStaticFiles(); //For images ,files

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
