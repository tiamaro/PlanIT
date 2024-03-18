using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Extensions;
using PlanIT.API.Mappers.Interface;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;
using Serilog;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester i beholderen (DI-containeren).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Tilpassede metoder for utvidelser
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IMapper<,>));    // Registrerer mappere
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IService<>));    // Registrerer services
builder.RegisterOpenGenericTypeAndDerivatives(typeof(IRepository<>)); // Registrerer repositories

// Registerer UserService
builder.Services.AddScoped<IUserService, UserService>();


// VALIDERING
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = false);


// Database-tilkobling via entityframework
builder.Services.AddDbContext<PlanITDbContext>(options =>
 options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
 new MySqlServerVersion(new Version(8, 0))));

// Tillegg for paginering
builder.Services.AddScoped<PaginationUtility>(); 

// Bruk av Serilog logger
builder.Host.UseSerilog((context, configuration) =>
{
    // Konfigurer logger fra appens konfigurasjonsfiler
    configuration.ReadFrom.Configuration(context.Configuration);
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();