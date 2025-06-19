// Services/User/IUserService.cs
// Interfaz para el servicio de gestión de usuarios.

using LibrosAutoresApi.Models; // Para referenciar la clase User
using LibrosAutoresApi.Dtos.Auth; // Para LoginDto y RegisterUserDto
using System.Threading.Tasks;

namespace LibrosAutoresApi.Services.User
{
    public interface IUserService
    {
        // Calificamos User con Models.User para evitar ambigüedad.
        Task<Models.User?> RegisterUser(RegisterUserDto registerDto);
        Task<Models.User?> ValidateUser(LoginDto loginDto);
        Task<Models.User?> GetUserByUsername(string username);
    }
}