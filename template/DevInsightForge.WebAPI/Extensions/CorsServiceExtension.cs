namespace DevInsightForge.WebAPI.Extensions;

public static class CorsServiceExtension
{
    public static IServiceCollection AddCorsService(this IServiceCollection services)
    {
        // Enforce CORS policy
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
