// Exceptions/NotFoundException.cs
// Excepción personalizada para cuando un recurso no se encuentra.

using System; // Necesario para la clase base Exception

namespace LibrosAutoresApi.Exceptions
{
    // Hereda de Exception. Por convención, las excepciones terminan en "Exception".
    public class NotFoundException : Exception
    {
        // Constructor que acepta un mensaje. Este mensaje será el que se muestre al cliente.
        public NotFoundException(string message) : base(message)
        {
        }

        // Puedes agregar otros constructores si necesitas pasar más información.
        // Este constructor, por ejemplo, permite encadenar excepciones (innerException).
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}