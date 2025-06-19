// Middlewares/ErrorHandlingMiddleware.cs
// Middleware para el manejo global de errores y excepciones en la API.

using Microsoft.AspNetCore.Http; // Para HttpContext, RequestDelegate
using Microsoft.Extensions.Logging; // Para ILogger
using System; // Para Exception
using System.Net; // Para HttpStatusCode
using System.Text.Json; // Para JsonSerializer
using System.Threading.Tasks; // Para Task
using LibrosAutoresApi.Exceptions; // Para usar tu excepción NotFoundException

namespace LibrosAutoresApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next; // Referencia al siguiente middleware en el pipeline
        private readonly ILogger<ErrorHandlingMiddleware> _logger; // Para registrar los errores

        // Constructor: inyección de dependencias del siguiente middleware y del logger.
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Método principal que se invoca para cada solicitud HTTP.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Intenta ejecutar el resto del pipeline de solicitudes.
                await _next(context);
            }
            catch (Exception ex) // Captura cualquier excepción no manejada.
            {
                // Registra la excepción para propósitos de depuración y monitoreo.
                _logger.LogError(ex, "Ha ocurrido una excepción no controlada: {Message}", ex.Message);

                // Llama a un método auxiliar para manejar la excepción y construir la respuesta.
                await HandleExceptionAsync(context, ex);
            }
        }

        // Método estático para procesar la excepción y generar una respuesta HTTP adecuada.
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Establece el tipo de contenido de la respuesta para indicar que será JSON.
            context.Response.ContentType = "application/json";

            // Valores por defecto para el código de estado y el mensaje de error.
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError; // 500
            string message = "Ha ocurrido un error interno en el servidor.";

            // Examina el tipo de excepción para determinar el código de estado HTTP y el mensaje.
            switch (exception)
            {
                // Si la excepción es de tipo NotFoundException, retornamos un 404.
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404 Not Found
                    message = notFoundException.Message; // Usamos el mensaje de la excepción personalizada.
                    break;
                // Puedes añadir más casos para otros tipos de excepciones si los creas,
                // por ejemplo, para ValidationException -> HttpStatusCode.BadRequest (400).
                // case ArgumentException argEx:
                //     statusCode = HttpStatusCode.BadRequest;
                //     message = argEx.Message;
                //     break;
            }

            // Establece el código de estado HTTP de la respuesta.
            context.Response.StatusCode = (int)statusCode;

            // Crea un objeto anónimo que representará el cuerpo JSON de la respuesta de error.
            var errorDetails = new
            {
                StatusCode = (int)statusCode,
                Message = message
            };

            // Serializa el objeto de error a JSON y lo escribe en el cuerpo de la respuesta HTTP.
            // JsonSerializer.Serialize requiere System.Text.Json;
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
        }
    }
}