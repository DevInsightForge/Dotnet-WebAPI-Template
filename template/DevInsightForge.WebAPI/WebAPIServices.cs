using DevInsightForge.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DevInsightForge.WebAPI;

public static class WebAPIServices
{
    public static void AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Inject Controller Handlers
        services.AddControllers();

        // Disable inbuild model validators in favor of Fluent Validation
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        // Inject Extension services
        services.AddAuthenticationService(configuration);
        services.AddSwaggerService();
        services.AddCorsService();

        services.AddExceptionHandler<ExceptionHandlerServiceExtension>();
        services.AddProblemDetails();
    }

    public static void UseWebAPIServices(this WebApplication app)
    {
        // Configure App Pipelines
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseSwaggerService();

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}


