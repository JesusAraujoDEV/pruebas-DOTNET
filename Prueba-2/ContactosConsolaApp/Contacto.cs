// Contacto.cs
// Define la estructura de un contacto para nuestra aplicación de agenda.

using System; // Necesario para usar DateTime

namespace ContactosConsolaApp
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; } // La fecha de nacimiento debe ser de tipo DateTime

        // Constructor para inicializar un nuevo Contacto con todos sus datos.
        public Contacto(int id, string nombre, string apellido, string telefono, string email, DateTime fechaNacimiento)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            Email = email;
            FechaNacimiento = fechaNacimiento;
        }

        // Sobrescribe el método ToString() para una representación legible del contacto en la consola.
        // El formato "dd/MM/yyyy" muestra la fecha como día/mes/año.
        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre} {Apellido}, Teléfono: {Telefono}, Email: {Email}, Fecha Nacimiento: {FechaNacimiento:dd/MM/yyyy}";
        }
    }
}
