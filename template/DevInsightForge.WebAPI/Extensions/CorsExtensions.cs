namespace DevInsightForge.WebAPI.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        return services;
    }
}
