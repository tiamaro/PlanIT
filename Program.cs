using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PlanIT.API.Configurations;
using PlanIT.API.Data;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Utilities;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Legg til tjenester i beholderen (DI-containeren).
builder.Services.AddScoped<HandleExceptionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Legger til tjenester fra utilities folder
builder.Services.AddScoped<PaginationUtility>();
builder.Services.AddScoped<LoggerService>();



// Tilpassede metoder for utvidelser
builder.Services.RegisterServicesFromConfiguration(Assembly.GetExecutingAssembly(), builder.Configuration);
builder.AddSwaggerWithJwtAuthentication(); // Registrerer swagger med jwt autentisering

//// background service 
//builder.Services.AddScoped<IHostedService, BackgroundWorkerService>();
builder.Services.AddHostedService<BackgroundWorkerService>();



// Registerer HttpContextAccessor
builder.Services.AddHttpContextAccessor();


// VALIDERING
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = false);


// Database-tilkobling via entityframework
builder.Services.AddDbContext<PlanITDbContext>(options =>
 options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
 new MySqlServerVersion(new Version(8, 0))));


// Legg til CORS-policy
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


// Bruk av Serilog logger
builder.Host.UseSerilog((context, configuration) =>
{
    // Konfigurer logger fra appens konfigurasjonsfiler
    configuration.ReadFrom.Configuration(context.Configuration);
});


// Tilpasset JWT-autentisering konfigurasjon
builder.Services.ConfigureAuthentication(builder.Configuration, Log.Logger);
builder.Services.ConfigureAuthorization();

var app = builder.Build();

// Konfigurer HTTP-forespørselsrørledningen.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

// app.UseMiddleware<GlobalExceptionMiddleware>(); // Global feilhåndtering
app.UseSerilogRequestLogging(); // Logger HTTP-forespørsler med Serilog

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowMyFrontend");

app.UseAuthentication(); // Setter opp autentiseringssystemet og etablerer brukeridentiteten

app.UseMiddleware<JWTClaimsValidationMiddleware>(); // Utfører tilpasset logikk basert på den etablerte brukeridentiteten

app.UseAuthorization(); // Håndterer autorisasjon basert på brukeridentiteten og tilhørende claims

app.MapControllers();

app.Run();