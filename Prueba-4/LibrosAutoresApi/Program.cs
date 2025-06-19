// Program.cs
// Archivo de inicio y configuración de la aplicación ASP.NET Core Web API.

using LibrosAutoresApi.Data;
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Services.Libro;
using LibrosAutoresApi.Services.Biografia;
using LibrosAutoresApi.Services.Evento;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization; // ¡NUEVO! Necesario para ReferenceHandler

var builder = WebApplication.CreateBuilder(args);

// **************** CONFIGURACIÓN DE SERVICIOS ****************

// Configura los controladores y el serializador JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ¡NUEVO! Configura el serializador JSON para manejar ciclos de referencia.
        // Esto previene el error "possible object cycle was detected".
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Opcional: Si quieres un formato más legible en el JSON de salida (camelCase por defecto)
        // options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });


// Configura DbContext para usar una base de datos en memoria.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("LibrosAutoresDb")); // Nombre de tu base de datos en memoria

// Registra tus servicios para la inyección de dependencias.
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IBiografiaService, BiografiaService>();
builder.Services.AddScoped<IEventoService, EventoService>();

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **************** CONSTRUCCIÓN DE LA APLICACIÓN ****************

var app = builder.Build();

// **************** INICIALIZACIÓN DE LA BASE DE DATOS (Solo para In-Memory) ****************
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); // Asegura que la base de datos en memoria se cree
}

// **************** CONFIGURACIÓN DEL PIPELINE DE SOLICITUDES HTTP ****************

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// **************** INICIO DE LA APLICACIÓN ****************

app.Run();