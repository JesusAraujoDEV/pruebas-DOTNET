// Program.cs
// Archivo de inicio y configuración de la aplicación ASP.NET Core Web API.
// ... otros usings existentes
using DotNetEnv; // ¡NUEVO!
using LibrosAutoresApi.Services.User; // ¡NUEVO! Para IUserService y UserService
using Microsoft.AspNetCore.Authentication.JwtBearer; // Necesario para JWT
using Microsoft.IdentityModel.Tokens; // Necesario para SymmetricSecurityKey
using System.Text; // Necesario para Encoding
using LibrosAutoresApi.Data; // Necesario para tu AppDbContext
using LibrosAutoresApi.Services.Autor;
using LibrosAutoresApi.Services.Libro;
using LibrosAutoresApi.Services.Biografia;
using LibrosAutoresApi.Services.Evento;
using Microsoft.EntityFrameworkCore; // Necesario para UseNpgsql y Database.Migrate()
using System.Text.Json.Serialization; // Necesario para ReferenceHandler
using LibrosAutoresApi.Middlewares; // ¡NUEVO! Para usar tu middleware de errores

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// **************** CONFIGURACIÓN DE SERVICIOS ****************

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configura DbContext para usar PostgreSQL.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra tus servicios.
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IBiografiaService, BiografiaService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IUserService, UserService>(); // ¡NUEVO! Registra tu servicio de usuarios.


// **************** CONFIGURACIÓN DE AUTENTICACIÓN JWT ****************
builder.Services.AddAuthentication(options =>
{
    // Establece el esquema de autenticación por defecto como JWT Bearer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Añade el manejador para tokens JWT Bearer.
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida que el emisor del token sea el esperado.
        ValidateAudience = true, // Valida que la audiencia del token sea la esperada.
        ValidateLifetime = true, // Valida que el token no haya expirado.
        ValidateIssuerSigningKey = true, // Valida la firma del token con la clave secreta.
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Lee el emisor de appsettings.json.
        ValidAudience = builder.Configuration["Jwt:Audience"], // Lee la audiencia de appsettings.json.

        // ¡CLAVE AQUÍ! Lee la clave secreta directamente de la variable de entorno JWT_SECRET_KEY.
        // builder.Configuration buscará esta variable de entorno.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET_KEY"]!))
    };
});

// Habilita el servicio de autorización.
builder.Services.AddAuthorization();


// **************** Configura Swagger/OpenAPI con soporte JWT ****************
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "LibrosAutoresApi", Version = "v1" });

    // Define el esquema de seguridad para JWT (Bearer token)
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization", // Nombre del encabezado HTTP para el token
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // Tipo de esquema de seguridad (HTTP)
        Scheme = "Bearer", // El esquema HTTP específico (Bearer)
        BearerFormat = "JWT", // Formato del token
        In = Microsoft.OpenApi.Models.ParameterLocation.Header, // Donde se envía el token (en el encabezado)
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer eyJhbGciOiJIUzI1Ni...\""
    });

    // Añade el requisito de seguridad para todos los endpoints documentados por Swagger
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer" // Debe coincidir con el nombre de AddSecurityDefinition
                }
            },
            new string[] {} // Indica que no hay roles específicos requeridos para este esquema general
        }
    });
});


// **************** CONSTRUCCIÓN DE LA APLICACIÓN ****************

var app = builder.Build();

// **************** APLICAR MIGRACIONES AL INICIO (Solo para desarrollo) ****************
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}


// **************** CONFIGURACIÓN DEL PIPELINE DE SOLICITUDES HTTP ****************

// ¡IMPORTANTE! El middleware de manejo de errores DEBE ir al PRINCIPIO.
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ¡IMPORTANTE! Estos dos DEBEN ir ANTES de app.MapControllers();
app.UseAuthentication(); // Habilita el middleware de autenticación (Lee el token, valida, establece el usuario en el contexto)
app.UseAuthorization();  // Habilita el middleware de autorización (Verifica los permisos del usuario establecido por Authentication)


app.MapControllers();

// **************** INICIO DE LA APLICACIÓN ****************

app.Run();