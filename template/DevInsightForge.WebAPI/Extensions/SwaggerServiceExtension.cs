using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace DevInsightForge.WebAPI.Extensions;

public static class SwaggerServiceExtension
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        // Inject Swagger/OpenAPI Service
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(setup =>
        {
            // Application Specifications
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DevInsightForge.API",
                Version = "v1",
                Description = "The DevInsightForge API built with ASP.NET Core, it ensures secure and efficient communication through JSON Web Tokens (JWT) for authentication."
            });
            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on the textbox below!"
            };

            var jwtSchemeId = JwtBearerDefaults.AuthenticationScheme;
            setup.AddSecurityDefinition(jwtSchemeId, jwtSecurityScheme);

            setup.AddSecurityRequirement(openApiDocument => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference(jwtSchemeId, openApiDocument, null), new List<string>() }
            });
        });

        return services;
    }
}
