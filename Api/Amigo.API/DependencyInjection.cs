using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Amigo.API;

public static class DependencyInjection
{
    public static IServiceCollection AddBasicDependencyInjcetion(this IServiceCollection services , IConfiguration configuration)
    {
        #region Basic
        services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()))
       .AddApplicationPart(typeof(Presentation.IAssemblyReference).Assembly);

        #endregion

        #region JWTBareerTokenConfigurations

        services.AddAuthentication(options =>
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
                ValidIssuer = configuration["JWTOptions:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JWTOptions:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWTOptions:SecretKey"])
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

        return services;
    }
}
