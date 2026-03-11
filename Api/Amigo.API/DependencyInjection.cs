namespace Amigo.API;

public static class DependencyInjection
{
    public static IServiceCollection AddBasicDependencyInjcetion(this IServiceCollection services , IConfiguration configuration)
    {
        #region Basic
        services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()))
       .AddApplicationPart(typeof(Presentation.IAssemblyReference).Assembly);

        #endregion
        return services;
    }
}
