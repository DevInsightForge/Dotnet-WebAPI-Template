using DevInsightForge.WebAPI.Contracts;
using DevInsightForge.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace DevInsightForge.WebAPI.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>() ??
            throw new InvalidOperationException("JwtConfiguration not defined");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = jwtConfiguration.ValidateAudience,
                ValidAudience = jwtConfiguration.ValidAudience,
                ValidateIssuer = jwtConfiguration.ValidateIssuer,
                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            options.Events = CreateJwtBearerEvents();
        });

        return services;
    }

    private static JwtBearerEvents CreateJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Type = "urn:devinsightforge:error:unauthorized",
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "Authentication failed. Please provide a valid access token.",
                    Instance = context.HttpContext.Request.Path
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Type = "urn:devinsightforge:error:forbidden",
                    Title = "Forbidden",
                    Status = (int)HttpStatusCode.Forbidden,
                    Detail = "You do not have permission to access this resource.",
                    Instance = context.HttpContext.Request.Path
                });
            }
        };
    }
}
