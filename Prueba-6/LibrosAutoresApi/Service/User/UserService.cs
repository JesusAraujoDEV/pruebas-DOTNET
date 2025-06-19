// Services/User/UserService.cs
// Implementación del servicio de gestión de usuarios.

using LibrosAutoresApi.Data; // Para AppDbContext
using LibrosAutoresApi.Models; // ¡NUEVO! Para referenciar la clase User (el modelo)
using LibrosAutoresApi.Dtos.Auth; // Para LoginDto, RegisterUserDto
using Microsoft.EntityFrameworkCore; // Para ToListAsync, FirstOrDefaultAsync
using Microsoft.Extensions.Logging; // Para ILogger
using System.Threading.Tasks;
using BCrypt.Net; // Para hashing de contraseñas

namespace LibrosAutoresApi.Services.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para registrar un nuevo usuario (incluye hashing de contraseña)
        public async Task<Models.User?> RegisterUser(RegisterUserDto registerDto) // Calificado con Models.User
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                _logger.LogWarning("Intento de registro con nombre de usuario existente: {Username}", registerDto.Username);
                return null;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var newUser = new Models.User // Calificado con Models.User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Role = "User"
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuario registrado exitosamente: {Username}", newUser.Username);
            return newUser;
        }

        // Método para validar las credenciales de login
        public async Task<Models.User?> ValidateUser(LoginDto loginDto) // Calificado con Models.User
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("Intento de login fallido: usuario {Username} no encontrado.", loginDto.Username);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Intento de login fallido: contraseña incorrecta para {Username}.", loginDto.Username);
                return null;
            }

            _logger.LogInformation("Usuario {Username} autenticado exitosamente.", loginDto.Username);
            return user;
        }

        // Método para obtener un usuario por nombre de usuario
        public async Task<Models.User?> GetUserByUsername(string username) // Calificado con Models.User
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}