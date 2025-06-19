// Program.cs
// Archivo de inicio y configuración de la aplicación ASP.NET Core Web API.

using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Services.Libro;
using LibrosAutoresApi.Services.Biografia; // <-- Nuevo
using LibrosAutoresApi.Services.Evento;   // <-- Nuevo

var builder = WebApplication.CreateBuilder(args);

// **************** CONFIGURACIÓN DE SERVICIOS ****************

builder.Services.AddControllers();

// Registro de los servicios existentes
builder.Services.AddSingleton<IAutorService, AutorService>();
builder.Services.AddSingleton<ILibroService, LibroService>();

// ¡Nuevos registros de servicios!
builder.Services.AddSingleton<IBiografiaService, BiografiaService>(); // <-- Nuevo
builder.Services.AddSingleton<IEventoService, EventoService>();     // <-- Nuevo

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **************** CONSTRUCCIÓN DE LA APLICACIÓN ****************

var app = builder.Build();

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