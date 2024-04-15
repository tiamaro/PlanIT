using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PlanIT.API.Configurations;
using PlanIT.API.Data;
using PlanIT.API.Extensions;
using PlanIT.API.Mappers.Interface;
using PlanIT.API.Middleware;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services;
using PlanIT.API.Services.AuthenticationService;
using PlanIT.API.Services.BackgroundWorkerServices;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Services.MailService;
using PlanIT.API.Utilities;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Registrer HandleExceptionFilter for controllers
builder.Services.AddScoped<HandleExceptionFilter>();

// Legg til tjenester i beholderen (DI-containeren).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


// Tilpassede metoder for utvidelser
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IMapper<,>));    // Registrerer mappere
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IService<>));    // Registrerer services
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IRepository<>)); // Registrerer repositories
builder.AddSwaggerWithJwtAuthentication(); // Registrerer swagger med jwt autentisering

// Registerer UserService
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthenticationService>();


// background service 
builder.Services.AddHostedService<BackgroundWorkerService>();


// Register HttpContextAccessor
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
                         .AllowAnyMethod();
        });
});


// Tillegg for paginering
builder.Services.AddScoped<PaginationUtility>();

// Bruk av Serilog logger
builder.Host.UseSerilog((context, configuration) =>
{
    // Konfigurer logger fra appens konfigurasjonsfiler
    configuration.ReadFrom.Configuration(context.Configuration);
});


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

app.UseCors("AllowMyFrontend");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Setter opp autentiseringssystemet og etablerer brukeridentiteten

app.UseMiddleware<JWTClaimsValidationMiddleware>(); // Utfører tilpasset logikk basert på den etablerte brukeridentiteten

app.UseAuthorization(); // Håndterer autorisasjon basert på brukeridentiteten og tilhørende claims

app.MapControllers();

app.Run();