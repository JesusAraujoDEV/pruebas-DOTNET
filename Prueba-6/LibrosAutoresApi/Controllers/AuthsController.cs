// Controllers/AuthsController.cs
// Controlador para el manejo de autenticación y generación de JWTs.

using Microsoft.AspNetCore.Mvc; // Para [ApiController], ControllerBase, ActionResult
using System.IdentityModel.Tokens.Jwt; // Para JwtSecurityTokenHandler
using System.Security.Claims; // Para Claims, ClaimsIdentity
using System.Text; // Para Encoding
using Microsoft.IdentityModel.Tokens; // Para SymmetricSecurityKey, SigningCredentials
using LibrosAutoresApi.Dtos.Auth; // Para tu LoginDto
using System.Threading.Tasks; // Para Task
using Microsoft.Extensions.Configuration; // Para IConfiguration (leer Jwt settings)
using System; // Para DateTime, Convert

namespace LibrosAutoresApi.Controllers
{
    // Este controlador no lleva [Authorize] porque su propósito es permitir la autenticación.
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/Auths
    public class AuthsController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Para acceder a las configuraciones de JWT (Key, Issuer, Audience, ExpireDays)

        public AuthsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST api/Auths/login
        // Endpoint para que un usuario se autentique y obtenga un token JWT.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto) // Recibe las credenciales en el cuerpo de la solicitud
        {
            // ***** Lógica de autenticación REAL aquí *****
            // En una aplicación real, no harías una verificación de usuario y contraseña quemada.
            // Consultarías una base de datos (ej. tabla Usuarios), verificarías el nombre de usuario,
            // hashearías la contraseña del DTO y la compararías con la contraseña hasheada almacenada.
            // Por simplicidad para esta prueba, usamos una simulación básica.
            if (loginDto.Username == "testuser" && loginDto.Password == "password123")
            {
                // Si las credenciales son válidas, generamos un JWT.
                var token = GenerateJwtToken(loginDto.Username);
                return Ok(new { Token = token }); // Retorna el token al cliente
            }

            // Si las credenciales son inválidas, retorna un 401 Unauthorized.
            return Unauthorized("Credenciales inválidas.");
        }

        // Método auxiliar privado para generar el token JWT.
        private string GenerateJwtToken(string username)
        {
            // Obtiene la clave secreta de las configuraciones (que leerá de tu variable de entorno JWT_SECRET_KEY)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define los "claims" (declaraciones) que se incluirán en el token.
            // Los claims son pares clave-valor que representan información sobre el sujeto del token.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username), // Identificador único del usuario
                new Claim(ClaimTypes.Name, username),           // Nombre del usuario
                new Claim(ClaimTypes.Role, "Admin")             // Ejemplo de rol (puedes tener múltiples roles)
                // new Claim("custom_claim_type", "custom_value") // Puedes añadir claims personalizados
            };

            // Crea el token JWT.
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"], // Emisor del token (ej. "TuApiLibrosAutores")
                _configuration["Jwt:Audience"], // Audiencia del token (ej. "UsuariosApi")
                claims, // Claims a incluir
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])), // Fecha de expiración
                signingCredentials: credentials); // Credenciales de firma

            // Escribe el token como una cadena y lo retorna.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}