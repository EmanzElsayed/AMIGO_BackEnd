
using Amigo.Domain.Entities;
using Amigo.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Amigo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddBasicDependencyInjcetion(builder.Configuration);


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AmigoDbContext>(options =>
            {

                options.UseNpgsql(connectionString);

            });
           

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            //{
            //    options.Password.RequiredLength = 8;
            //    options.User.RequireUniqueEmail = true;

            //    //blocks login if email not confirmed
            //    options.SignIn.RequireConfirmedEmail = true;

            //    //If a user enters the wrong password 5 times, their account becomes locked.
            //    options.Lockout.MaxFailedAccessAttempts = 5;

            //    //How long the user stays locked out:
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

            //}).AddEntityFrameworkStores<AmigoDbContext>()
            //  .AddDefaultTokenProviders();

            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
