// Controllers/AuthsController.cs
// Controlador para el manejo de autenticación y generación de JWTs.

using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; // Para JwtSecurityTokenHandler
using System.Security.Claims; // Para Claims
using System.Text; // Para Encoding
using Microsoft.IdentityModel.Tokens; // Para SymmetricSecurityKey
using LibrosAutoresApi.Dtos.Auth; // Para tu LoginDto y RegisterUserDto
using System.Threading.Tasks; // Para Task
using Microsoft.Extensions.Configuration; // Para IConfiguration (leer Jwt settings)
using System; // Para DateTime, Convert

using LibrosAutoresApi.Services.User; // Para IUserService
using LibrosAutoresApi.Models; // Para la clase User (el modelo)

// Eliminar cualquier 'using Microsoft.AspNetCore.Authorization;' duplicado aquí si lo tenías.

namespace LibrosAutoresApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthsController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        // POST api/Auths/register
        // Endpoint para registrar un nuevo usuario.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var newUser = await _userService.RegisterUser(registerDto);

            if (newUser == null)
            {
                // Podrías retornar un BadRequest si el nombre de usuario ya existe
                return BadRequest("El nombre de usuario ya existe o hubo un error al registrar.");
            }

            // Generar un token para el usuario recién registrado
            var token = GenerateJwtToken(newUser); // ¡CORREGIDO! Pasar el objeto Models.User
            return StatusCode(201, new { Message = "Usuario registrado exitosamente.", Token = token }); // 201 Created
        }

        // POST api/Auths/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Usamos el UserService para validar las credenciales
            var user = await _userService.ValidateUser(loginDto); // Aquí obtenemos el objeto Models.User o null

            if (user == null)
            {
                // Si ValidateUser retorna null, las credenciales son inválidas
                return Unauthorized("Credenciales inválidas."); // HTTP 401
            }

            // Si el usuario es válido, generamos el JWT incluyendo su rol.
            var token = GenerateJwtToken(user); // ¡CORREGIDO! Pasar el objeto Models.User
            return Ok(new { Token = token });
        }

        // Método auxiliar privado para generar el token JWT.
        // Acepta un objeto Models.User para incluir el rol y otras claims.
        private string GenerateJwtToken(Models.User user) // ¡AQUÍ ESTÁ LA FIRMA QUE ESPERA UN Models.User!
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID de usuario
                new Claim(ClaimTypes.Name, user.Username),               // Nombre de usuario
                new Claim(ClaimTypes.Role, user.Role)                   // ¡IMPORTANTE! Rol del usuario
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}