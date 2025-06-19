using System;
using System.Collections.Generic; // Para usar List<T>
using System.IO;                   // Para usar File.Exists, File.ReadAllLines, File.WriteAllLines
using System.Linq;                 // Para usar métodos como .Any() o .Max() en colecciones


// See https://aka.ms/new-console-template for more information

namespace InventarioConsolaApp // Si tu plantilla usa namespace, manténlo. Si no, quítalo.
{
    public class Program // Si tu plantilla usa class Program, manténla. Si no, quítala.
    {
        // Esta lista guardará nuestros productos en memoria mientras la app está corriendo
        private static List<Producto> inventario = new List<Producto>();

        // Ruta del archivo donde guardaremos/cargaremos los datos.
        // Environment.CurrentDirectory asegura que el archivo se guarde junto al ejecutable de la app.
        private const string RUTA_ARCHIVO = "inventario.txt";

        // ... (aquí irá el método Main y los demás métodos de la aplicación)

        static void Main(string[] args)
        {
            // *** PASO CLAVE 1: Cargar inventario al inicio ***
            // Intentar cargar cualquier dato de inventario previamente guardado desde el archivo.
            CargarInventarioDesdeArchivo();

            bool ejecutarApp = true;
            while (ejecutarApp) // Bucle principal: la aplicación se ejecuta hasta que el usuario decida salir
            {
                MostrarMenu(); // Muestra las opciones al usuario
                string opcion = Console.ReadLine(); // Lee la entrada del usuario

                switch (opcion) // Evalúa la opción elegida por el usuario
                {
                    case "1": // Opción para agregar un producto
                        AgregarProducto();
                        break;
                    case "2": // Opción para ver todos los productos
                        VerTodosLosProductos();
                        break;
                    case "3": // Opción para eliminar producto (no implementada en este ejercicio base)
                        Console.WriteLine("La opción 'Eliminar Producto' no está implementada en este ejercicio base.");
                        break;
                    case "4": // Opción para salir de la aplicación
                        ejecutarApp = false; // Cambia la bandera para terminar el bucle
                        // *** PASO CLAVE 2: Guardar inventario al salir ***
                        // Guardar todos los cambios del inventario en el archivo antes de cerrar la app.
                        GuardarInventarioAArchivo();
                        Console.WriteLine("Saliendo de la aplicación. ¡Hasta pronto!");
                        break;
                    default: // Si la opción no es válida
                        Console.WriteLine("Opción inválida. Por favor, intente de nuevo.");
                        break;
                }

                // Pausa para que el usuario pueda leer los mensajes antes de que el menú se muestre de nuevo
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey(); // Espera a que el usuario presione una tecla
            }
        }

        static void MostrarMenu()
        {
            Console.Clear(); // Limpia la consola para una pantalla más ordenada
            Console.WriteLine("--- Sistema de Gestión de Inventario ---");
            Console.WriteLine("1. Agregar Producto");
            Console.WriteLine("2. Ver Todos los Productos");
            Console.WriteLine("3. Eliminar Producto (Opcional - No implementado en este ejercicio)");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static void AgregarProducto()
        {
            Console.WriteLine("\n--- Agregar Nuevo Producto ---");

            // Genera un ID simple y ascendente.
            // Si el inventario está vacío, el primer ID será 1.
            // Si ya hay productos, toma el ID máximo existente y le suma 1.
            int nuevoId = inventario.Any() ? inventario.Max(p => p.Id) + 1 : 1;

            Console.Write("Ingrese el Nombre del Producto: ");
            string nombre = Console.ReadLine(); // Lee el nombre del producto

            double precio;
            Console.Write("Ingrese el Precio del Producto: ");
            // Loop para asegurarse de que el usuario ingrese un precio válido (número positivo)
            while (!double.TryParse(Console.ReadLine(), out precio) || precio < 0)
            {
                Console.WriteLine("Precio inválido. Por favor, ingrese un número positivo para el precio:");
            }

            int cantidad;
            Console.Write("Ingrese la Cantidad en Stock: ");
            // Loop para asegurarse de que el usuario ingrese una cantidad válida (entero positivo)
            while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad < 0)
            {
                Console.WriteLine("Cantidad inválida. Por favor, ingrese un número entero positivo para la cantidad:");
            }

            // Crea una nueva instancia de Producto con los datos ingresados
            Producto nuevoProducto = new Producto(nuevoId, nombre, precio, cantidad);
            inventario.Add(nuevoProducto); // Agrega el nuevo producto a la lista en memoria

            Console.WriteLine($"\nProducto '{nombre}' (ID: {nuevoId}) agregado al inventario.");
        }

        static void VerTodosLosProductos()
        {
            Console.WriteLine("\n--- Productos en Inventario ---");
            if (inventario.Count == 0) // Verifica si la lista está vacía
            {
                Console.WriteLine("No hay productos en el inventario. Agregue algunos primero.");
                return; // Sale del método si no hay productos que mostrar
            }

            foreach (var producto in inventario) // Itera sobre cada producto en la lista 'inventario'
            {
                Console.WriteLine(producto); // Esto llama automáticamente al método ToString() de la clase Producto
            }
        }

        static void GuardarInventarioAArchivo()
        {
            try
            {
                List<string> lineasParaGuardar = new List<string>();
                foreach (var producto in inventario)
                {
                    // Formato CSV para cada línea: ID,Nombre,Precio,Cantidad
                    // Los números se convertirán a texto automáticamente
                    lineasParaGuardar.Add($"{producto.Id},{producto.Nombre},{producto.Precio},{producto.Cantidad}");
                }

                // Escribe todas las líneas en el archivo especificado por RUTA_ARCHIVO.
                // Si el archivo no existe, lo crea. Si ya existe, su contenido anterior será sobrescrito.
                File.WriteAllLines(RUTA_ARCHIVO, lineasParaGuardar);
                Console.WriteLine($"\nInventario guardado en '{RUTA_ARCHIVO}' exitosamente.");
            }
            catch (Exception ex) // Captura cualquier error que pueda ocurrir durante la operación de escritura del archivo
            {
                Console.WriteLine($"\n¡Error al guardar el inventario! Mensaje: {ex.Message}");
            }
        }

        static void CargarInventarioDesdeArchivo()
        {
            // Primero, verifica si el archivo de inventario existe en la ruta especificada.
            if (!File.Exists(RUTA_ARCHIVO))
            {
                Console.WriteLine("Archivo de inventario no encontrado. La aplicación iniciará con un inventario vacío.");
                return; // No hay nada que cargar, así que salimos del método.
            }

            try
            {
                // Lee todas las líneas del archivo y las guarda en un array de strings.
                string[] lineas = File.ReadAllLines(RUTA_ARCHIVO);

                inventario.Clear(); // Limpia la lista en memoria para evitar duplicar productos si ya había algo cargado.

                foreach (string linea in lineas)
                {
                    // Divide cada línea en partes usando la coma como delimitador.
                    string[] partes = linea.Split(',');

                    // Asegúrate de que la línea tiene el número correcto de partes para un Producto.
                    if (partes.Length == 4)
                    {
                        // Intenta convertir las partes a los tipos de datos correctos (int, double, int).
                        // TryParse es seguro: devuelve true si la conversión es exitosa y false si falla.
                        if (int.TryParse(partes[0], out int id) &&
                            double.TryParse(partes[2], out double precio) &&
                            int.TryParse(partes[3], out int cantidad))
                        {
                            string nombre = partes[1]; // El nombre no necesita parseo, ya es un string.
                            inventario.Add(new Producto(id, nombre, precio, cantidad)); // Crea un nuevo Producto y lo añade a la lista.
                        }
                        else
                        {
                            // Si alguna parte numérica no se pudo parsear, lo indicamos.
                            Console.WriteLine($"Advertencia: No se pudo leer una línea del archivo correctamente (datos numéricos inválidos): {linea}");
                        }
                    }
                    else
                    {
                        // Si la línea no tiene 4 partes, su formato es incorrecto.
                        Console.WriteLine($"Advertencia: Formato de línea inválido en el archivo (número incorrecto de elementos): {linea}");
                    }
                }
                Console.WriteLine($"\nInventario cargado exitosamente desde '{RUTA_ARCHIVO}'. Se cargaron {inventario.Count} productos.");
            }
            catch (Exception ex) // Captura cualquier error general que pueda ocurrir durante la lectura del archivo
            {
                Console.WriteLine($"\n¡Error al cargar el inventario! Mensaje: {ex.Message}");
            }
        }

    }
}