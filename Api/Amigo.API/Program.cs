
using Amigo.API.MiddleWare;
using Amigo.Application;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.BackgroundTasks;
using Amigo.Application.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Entities;
using Amigo.Infrastructure;
using Amigo.Persistence;
using Amigo.Persistence.Services;
using Amigo.Presentation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
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
                            .AddApplicationDependencyInjection(builder.Configuration)
                            .AddPresentationDependencyInjection(builder.Configuration)
                            .AddInfrastructureDependencyInjection(builder.Configuration);
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddScoped<ITopDestinationsReader, TopDestinationsReader>();
            builder.Services.AddSingleton<IBackgroundTaskQueue>(
             _ => new BackgroundTaskQueue(capacity: 200));
            builder.Services.AddHostedService<TranslationWorkerService>();
            builder.Services.AddHttpClient<ITranslationService, GeminiTranslationService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60); 
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            StripeConfiguration.ApiKey =
                    builder.Configuration["Stripe:SecretKey"];

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
            builder.Services.AddProblemDetails();

            var app = builder.Build();
            app.UseDeveloperExceptionPage();

            #region Localization
            var localizationOptions =
                     app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();

            //app.UseRequestLocalization(localizationOptions.Value);

            #endregion


            #region SeedingData

            using var scope = app.Services.CreateScope();
            var seedObj = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await seedObj.IdentityDataSeedAsync();


            #endregion

            app.UseMiddleware<GlobalExceptionMiddleware>();
           
            app.UseMiddleware<CultureMiddleware>();

            app.UseRateLimiter();
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
//Background Job:
/*  1)
 *  
    PendingPayment older than 30 mins
    → Expire Order
    → Release Slots
 

2)
var expired = Reservations
    .Where(x => x.Status == Pending &&
                x.ExpiresAt < DateTime.UtcNow);
 */