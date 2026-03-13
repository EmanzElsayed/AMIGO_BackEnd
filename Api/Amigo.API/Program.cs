
using Amigo.API.MiddleWareExceptions;
using Amigo.Application;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Entities;
using Amigo.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
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
            

            builder.Services.AddScoped< IEmailService,GoogleEmailService>();
            builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();



            #region JWTBareerTokenConfigurations

            builder.Services.AddAuthentication(options =>
            {
                // Default scheme for normal JWTs
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"])
                    ),
                    ValidateIssuerSigningKey = true,
                    // Map claim types
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    // 401 - Not authenticated
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            message = "Authentication required"
                        };

                        return context.Response.WriteAsJsonAsync(response);
                    },

                    // 403 - Authenticated but not authorized (wrong role)
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            message = "You are not authorized to access this resource"
                        };

                        return context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

            #endregion


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
