// Models/User.cs
// Modelo para representar un usuario del sistema.

using System.ComponentModel.DataAnnotations; // Para [Key]
using System.ComponentModel.DataAnnotations.Schema; // Para [Table]

namespace LibrosAutoresApi.Models
{
    [Table("Users")] // Opcional: define explícitamente el nombre de la tabla en la DB
    public class User
    {
        [Key] // Marca Id como la clave primaria.
        public int Id { get; set; }

        [Required] // Campo obligatorio.
        [MaxLength(50)] // Longitud máxima para el nombre de usuario.
        public required string Username { get; set; }

        [Required]
        // Esta propiedad almacenará el hash de la contraseña, no la contraseña en texto plano.
        public required string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)] // Ej. "Admin", "User", "Editor"
        public required string Role { get; set; } = "User"; // Valor por defecto
    }
}