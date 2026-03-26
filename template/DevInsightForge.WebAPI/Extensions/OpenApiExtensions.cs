using DevInsightForge.WebAPI.Contracts;
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
            options.AddDocumentTransformer<DefaultDocumentTransformer>();
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

public sealed class DefaultDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Web API",
            Version = "v1",
            Description = "Generic ASP.NET Core Web API template."
        };

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

