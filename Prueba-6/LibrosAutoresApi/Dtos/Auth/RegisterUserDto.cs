// Dtos/Auth/RegisterUserDto.cs
// DTO para los datos de registro de un nuevo usuario.

using System.ComponentModel.DataAnnotations;

namespace LibrosAutoresApi.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)] // Indica que es un campo de contraseña
        public required string Password { get; set; }
    }
}