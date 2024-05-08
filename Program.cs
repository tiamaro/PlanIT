using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PlanIT.API.Configurations;
using PlanIT.API.Data;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;
using Serilog;
using System.Reflection;
using PlanIT.API.Services.AuthenticationService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container (DI container).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<HandleExceptionFilter>();
builder.Services.AddTransient<GlobalExceptionMiddleware>(); // Transient: Opprettes for hver anmodning

// Add services from the utilities folder
builder.Services.AddScoped<PaginationUtility>();
builder.Services.AddScoped<LoggerService>();
builder.Services.AddSingleton<ILoggerServiceFactory, LoggerServiceFactory>();


// Custom extension methods
builder.Services.RegisterServicesFromConfiguration(Assembly.GetExecutingAssembly(), builder.Configuration);
builder.AddSwaggerWithJwtAuthentication(); // Registrerer swagger med jwt autentisering

// Background service
builder.Services.AddHostedService<BackgroundWorkerService>();

// Email authentiaction service
builder.Services.AddSingleton<IEmailAuth, EmailAuthService>();

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();


// Data validation using FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = false);


// Database connection via Entity Framework
builder.Services.AddDbContext<PlanITDbContext>(options =>
 options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
 new MySqlServerVersion(new Version(8, 0))));


// Add CORS policy (for frontend testing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyFrontend",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://127.0.0.1:5500")
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
        });
});


// Use Serilog logger
builder.Host.UseSerilog((context, configuration) =>
{
    // Configure logger from the app's configuration files
    configuration.ReadFrom.Configuration(context.Configuration);
});


// Configure SmtpSettings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<SmtpClientFactory>();


// Custom JWT authentication configuration
builder.Services.ConfigureAuthentication(builder.Configuration, Log.Logger);
builder.Services.ConfigureAuthorization();


var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseMiddleware<GlobalExceptionMiddleware>(); 
app.UseSerilogRequestLogging(); 

app.UseHttpsRedirection();

app.UseRouting();

//
app.UseCors("AllowMyFrontend");

// Set up authentication system and establish user identity
app.UseAuthentication();

// Execute custom logic based on established user identity
app.UseMiddleware<JWTClaimsValidationMiddleware>();

// Handle authorization based on user identity and associated claims
app.UseAuthorization(); 


app.MapControllers();

app.Run();

// For running integration tests
public partial class Program { }