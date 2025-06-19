// Dtos/Auth/LoginDto.cs
// DTO para las credenciales de login.

using System.ComponentModel.DataAnnotations; // Para atributos como [Required]

namespace LibrosAutoresApi.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public required string Username { get; set; } // 'required' en C# 11+ asegura que debe ser inicializado

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public required string Password { get; set; }
    }
}