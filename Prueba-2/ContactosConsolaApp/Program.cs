// Program.cs
// Este archivo contiene la lógica principal de la aplicación de gestión de contactos.

using System;
using System.Collections.Generic; // Para usar List<T>
using System.IO;                   // Para usar File.Exists, File.ReadAllLines, File.WriteAllLines
using System.Linq;                 // Para usar métodos de LINQ como .Any(), .Max(), .Where()
using System.Globalization;        // Necesario para CultureInfo.InvariantCulture en el parseo de fechas
// Agrega System.Text.RegularExpressions si vas a usar expresiones regulares para validar email/telefono,
// aunque para este ejercicio no es estrictamente necesario todavía.

namespace ContactosConsolaApp
{
    public class Program
    {
        // Lista estática para guardar los contactos en memoria mientras la aplicación se ejecuta.
        private static List<Contacto> agenda = new List<Contacto>();

        // Ruta del archivo donde se guardarán y cargarán los datos de la agenda.
        private const string RUTA_ARCHIVO = "contactos.txt";

        // **************** MÉTODO PRINCIPAL (PUNTO DE ENTRADA) ****************
        // Este es el método Main, que es lo primero que se ejecuta al iniciar la aplicación.
        static void Main(string[] args)
        {
            // Intentar cargar los contactos guardados desde el archivo al inicio de la aplicación.
            CargarAgendaDesdeArchivo();

            bool ejecutarApp = true;
            while (ejecutarApp) // Bucle principal del menú: la aplicación se ejecuta hasta que el usuario decida salir.
            {
                MostrarMenu(); // Muestra las opciones al usuario.
                string opcion = Console.ReadLine(); // Lee la opción ingresada por el usuario.

                switch (opcion) // Evalúa la opción y ejecuta la función correspondiente.
                {
                    case "1":
                        AgregarContacto(); // Llama a la función para agregar un contacto.
                        break;
                    case "2":
                        VerTodosLosContactos(); // Llama a la función para ver todos los contactos.
                        break;
                    case "3":
                        BuscarContacto(); // Llama a la función para buscar un contacto.
                        break;
                    case "4":
                        ejecutarApp = false; // Establece la bandera para salir del bucle.
                        GuardaAgendaAArchivo(); // Guarda la agenda antes de salir.
                        Console.WriteLine("\nSaliendo de la aplicación. ¡Hasta pronto!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Por favor, intente de nuevo.");
                        break;
                }

                // Pausa para que el usuario pueda leer los mensajes antes de que el menú se muestre de nuevo.
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey(); // Espera a que el usuario presione una tecla.
            }
        }

        // **************** MÉTODOS AUXILIARES Y LÓGICA DE NEGOCIO ****************

        // Muestra el menú de opciones en la consola.
        static void MostrarMenu()
        {
            Console.Clear(); // Limpia la consola para una pantalla más ordenada.
            Console.WriteLine("--- Sistema de Gestión de Contactos ---");
            Console.WriteLine("1. Agregar Nuevo Contacto");
            Console.WriteLine("2. Ver Todos los Contactos");
            Console.WriteLine("3. Buscar Contacto por Nombre o Apellido");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
        }

        // Permite al usuario agregar un nuevo contacto a la agenda.
        static void AgregarContacto() // Nombre de método corregido: de AgregarProducto a AgregarContacto
        {
            Console.WriteLine("\n--- Agregar Nuevo Contacto ---");

            // Genera un ID único y ascendente para el nuevo contacto.
            // Si la agenda está vacía, el primer ID será 1.
            // Si ya hay contactos, toma el ID máximo existente y le suma 1.
            int nuevoId = agenda.Any() ? agenda.Max(c => c.Id) + 1 : 1; // Corregido: p => p.Id a c => c.Id

            Console.Write("Ingrese el Nombre del Contacto: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese el Apellido del Contacto: ");
            string apellido = Console.ReadLine();

            Console.Write("Ingrese el Número de Teléfono del Contacto: ");
            string telefono = Console.ReadLine();

            Console.Write("Ingrese el Email del Contacto: ");
            string email = Console.ReadLine();

            DateTime fechaNacimiento;
            Console.Write("Ingrese la fecha de nacimiento (DD/MM/YYYY): ");
            // Loop para validar que la fecha de nacimiento sea en el formato "DD/MM/YYYY" y sea una fecha válida.
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimiento))
            {
                Console.WriteLine("Fecha inválida. Por favor, use el formato DD/MM/YYYY:");
            }

            // Crea una nueva instancia de Contacto con los datos ingresados.
            Contacto nuevoContacto = new Contacto(nuevoId, nombre, apellido, telefono, email, fechaNacimiento); // Tipo corregido: de Producto a Contacto
            agenda.Add(nuevoContacto); // Agrega el nuevo contacto a la lista en memoria.

            Console.WriteLine($"\nEl contacto '{nombre} {apellido}' (ID: {nuevoId}) ha sido agregado a la agenda."); // Corregido: \El a \nEl, y mensaje.
        }

        // Muestra todos los contactos actualmente en la agenda.
        static void VerTodosLosContactos()
        {
            Console.WriteLine("\n--- Contactos en la Agenda ---");
            if (agenda.Count == 0) // Verifica si la agenda está vacía.
            {
                Console.WriteLine("No hay contactos en la agenda. Agregue algunos primero.");
                return; // Sale del método si no hay contactos que mostrar.
            }

            foreach (var contacto in agenda) // Itera sobre cada contacto en la lista 'agenda'.
            {
                Console.WriteLine(contacto); // Llama implícitamente al método ToString() de la clase Contacto (comentario actualizado).
            }
        }

        // Permite al usuario buscar contactos por nombre o apellido.
        static void BuscarContacto()
        {
            Console.WriteLine("\n--- Buscar Contacto ---");
            if (agenda.Count == 0)
            {
                Console.WriteLine("La agenda está vacía. No hay contactos para buscar.");
                return;
            }

            Console.Write("Ingrese el nombre o apellido a buscar: ");
            string terminoBusqueda = Console.ReadLine();

            // Valida que el término de búsqueda no esté vacío o sea solo espacios.
            if (string.IsNullOrWhiteSpace(terminoBusqueda))
            {
                Console.WriteLine("El término de búsqueda no puede estar vacío.");
                return;
            }

            // Realiza la búsqueda insensible a mayúsculas/minúsculas usando LINQ.
            // StringComparison.OrdinalIgnoreCase es crucial para una comparación que ignora mayúsculas/minúsculas.
            var resultados = agenda.Where(c => c.Nombre.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                                              c.Apellido.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase))
                                   .ToList(); // Convierte los resultados filtrados a una nueva lista.

            if (resultados.Any()) // Si se encontraron resultados.
            {
                Console.WriteLine($"\n--- Resultados de la búsqueda para '{terminoBusqueda}' ---");
                foreach (var contacto in resultados)
                {
                    Console.WriteLine(contacto);
                }
            }
            else // Si no se encontraron resultados.
            {
                Console.WriteLine($"No se encontraron contactos que coincidan con '{terminoBusqueda}'.");
            }
        }

        // Guarda la agenda actual en un archivo de texto.
        static void GuardaAgendaAArchivo()
        {
            try
            {
                List<string> lineasParaGuardar = new List<string>();
                foreach (var contacto in agenda)
                {
                    // Formato de cada línea: ID|Nombre|Apellido|Telefono|Email|FechaNacimiento (formato YYYY-MM-DD)
                    // Usamos '|' como delimitador para evitar problemas si algún campo (como el nombre o email) contiene comas.
                    lineasParaGuardar.Add($"{contacto.Id}|{contacto.Nombre}|{contacto.Apellido}|{contacto.Telefono}|{contacto.Email}|{contacto.FechaNacimiento:yyyy-MM-dd}");
                }

                // Escribe todas las líneas en el archivo especificado por RUTA_ARCHIVO.
                // Si el archivo no existe, lo crea; si existe, su contenido anterior será sobrescrito.
                File.WriteAllLines(RUTA_ARCHIVO, lineasParaGuardar);
                Console.WriteLine($"\nAgenda guardada en '{RUTA_ARCHIVO}' exitosamente.");
            }
            catch (Exception ex) // Captura cualquier error durante la operación de escritura del archivo.
            {
                Console.WriteLine($"\n¡Error al guardar la agenda! Mensaje: {ex.Message}");
            }
        }

        // Carga la agenda desde un archivo de texto al inicio de la aplicación.
        static void CargarAgendaDesdeArchivo()
        {
            // Primero, verifica si el archivo de la agenda existe en la ruta especificada.
            if (!File.Exists(RUTA_ARCHIVO))
            {
                Console.WriteLine("Archivo de agenda no encontrado. La aplicación iniciará con una agenda vacía.");
                return; // Sale del método si no hay archivo.
            }

            try
            {
                string[] lineas = File.ReadAllLines(RUTA_ARCHIVO); // Lee todas las líneas del archivo.
                agenda.Clear(); // Limpia la lista en memoria antes de cargar para evitar duplicados.

                foreach (string linea in lineas)
                {
                    // Divide la línea en partes usando el delimitador '|'.
                    string[] partes = linea.Split('|');

                    // Asegúrate de que la línea tiene las 6 partes esperadas para un Contacto.
                    if (partes.Length == 6)
                    {
                        int id;
                        DateTime fechaNacimiento;

                        // Intenta convertir las partes a los tipos de datos correctos (int para ID y DateTime para FechaNacimiento).
                        // TryParse es seguro: devuelve true si la conversión es exitosa y false si falla.
                        if (int.TryParse(partes[0], out id) && // Parsea el ID
                            DateTime.TryParseExact(partes[5], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimiento)) // Parsea la fecha
                        {
                            string nombre = partes[1];
                            string apellido = partes[2];
                            string telefono = partes[3];
                            string email = partes[4];

                            // Crea un nuevo objeto Contacto y lo añade a la lista 'agenda'.
                            agenda.Add(new Contacto(id, nombre, apellido, telefono, email, fechaNacimiento));
                        }
                        else
                        {
                            // Si alguna parte (ID o Fecha) no se pudo parsear, lo indicamos al usuario.
                            Console.WriteLine($"Advertencia: No se pudo leer una línea del archivo correctamente (datos numéricos o de fecha inválidos): {linea}");
                        }
                    }
                    else
                    {
                        // Si la línea no tiene el número correcto de partes, su formato es incorrecto.
                        Console.WriteLine($"Advertencia: Formato de línea inválido en el archivo (número incorrecto de elementos): {linea}");
                    }
                }
                // Mensaje final de carga.
                Console.WriteLine($"\nAgenda cargada exitosamente desde '{RUTA_ARCHIVO}'. Se cargaron {agenda.Count} contactos.");
            }
            catch (Exception ex) // Captura cualquier error general que pueda ocurrir durante la lectura del archivo.
            {
                Console.WriteLine($"\n¡Error al cargar la agenda! Mensaje: {ex.Message}");
            }
        }
    }
}
