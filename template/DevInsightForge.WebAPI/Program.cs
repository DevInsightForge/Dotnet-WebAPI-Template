using DevInsightForge.Application;
using DevInsightForge.Infrastructure;
using DevInsightForge.Persistence;
using DevInsightForge.WebAPI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog to container
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

// Initialize app from builder
var app = builder.Build();
app.UseWebApiServices();

await app.RunAsync();

