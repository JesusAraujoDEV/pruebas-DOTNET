// Program.cs
// Archivo de inicio y configuración de la aplicación ASP.NET Core Web API.

using LibrosAutoresApi.Data; // Necesario para tu AppDbContext
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Services.Libro;
using LibrosAutoresApi.Services.Biografia;
using LibrosAutoresApi.Services.Evento;
using Microsoft.EntityFrameworkCore; // Necesario para UseNpgsql y Database.Migrate()
using System.Text.Json.Serialization; // Necesario para ReferenceHandler

var builder = WebApplication.CreateBuilder(args);

// **************** CONFIGURACIÓN DE SERVICIOS ****************

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ¡CAMBIO CLAVE! Configura DbContext para usar PostgreSQL.
// Obtiene la cadena de conexión del archivo appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registra tus servicios para la inyección de dependencias.
// Siguen siendo AddScoped.
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IBiografiaService, BiografiaService>();
builder.Services.AddScoped<IEventoService, EventoService>();

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **************** CONSTRUCCIÓN DE LA APLICACIÓN ****************

var app = builder.Build();

// **************** APLICAR MIGRACIONES AL INICIO (Solo para desarrollo) ****************
// En producción, es mejor ejecutar las migraciones como un paso separado
// en tu proceso de despliegue. Aquí lo hacemos al inicio para conveniencia de desarrollo.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.Migrate(); // Aplica las migraciones pendientes a la base de datos
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            // Aquí podrías manejar el error, por ejemplo, detener la aplicación o registrarlo.
        }
    }
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