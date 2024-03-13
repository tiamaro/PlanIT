using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Extensions;
using PlanIT.API.Mappers.Interface;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester i beholderen (DI-containeren).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Tilpassede metoder for utvidelser
builder.RegisterOpenGenericType(typeof(IMapper<,>)); // Registrerer mappere
// builder.RegisterOpenGenericType(typeof(IRepository<,>)); // Registrerer repositories
// builder.RegisterOpenGenericType(typeof(IService<,>)); // Registrerer services


// Database-tilkobling via entityframework
builder.Services.AddDbContext<PlanITDbContext>(options =>
 options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
 new MySqlServerVersion(new Version(8, 0))));

var app = builder.Build();

// Konfigurer HTTP-forespørselsrørledningen.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
