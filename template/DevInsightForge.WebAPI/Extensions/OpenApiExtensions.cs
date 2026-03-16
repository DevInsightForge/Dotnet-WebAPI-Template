using DevInsightForge.WebAPI.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DevInsightForge.WebAPI.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<JwtBearerSecurityDocumentTransformer>();
            options.AddOperationTransformer<ProblemDetailsOperationTransformer>();
        });

        return services;
    }

    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
            options.EnablePersistAuthorization();
            options.DisplayRequestDuration();
            options.EnableTryItOutByDefault();
            options.EnableFilter();
            options.DocExpansion(DocExpansion.List);
            options.DefaultModelsExpandDepth(0);
        });
    }
}

public sealed class JwtBearerSecurityDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string SecuritySchemeId = JwtBearerDefaults.AuthenticationScheme;

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "DevInsightForge.API",
            Version = "v1",
            Description = "The DevInsightForge API built with ASP.NET Core, it ensures secure and efficient communication through JSON Web Tokens (JWT) for authentication."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SecuritySchemeId] = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Put **_ONLY_** your JWT Bearer token on the textbox below!"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeId, document, null)] = []
        });

        return Task.CompletedTask;
    }
}

public sealed class ProblemDetailsOperationTransformer : IOpenApiOperationTransformer
{
    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Responses ??= [];

        var errorResponseSchema = await context.GetOrCreateSchemaAsync(typeof(ErrorResponse), null, cancellationToken);

        operation.Responses["4xx/5xx"] = new OpenApiResponse
        {
            Description = typeof(ErrorResponse).Name,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = errorResponseSchema
                }
            }
        };

        return;
    }
}
