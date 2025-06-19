// Program.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// Agrega esta línea para poder usar la clase EmailExtractor
using ExtractorConsoleApp.Utilities; // O el namespace que le hayas dado a EmailExtractor.cs

// El resto del código de Program.cs está en el ámbito global (top-level statements)

// --- Código de prueba para EmailExtractor ---

// Crea un archivo input.txt en la misma carpeta que tu ejecutable (.exe)
string inputFilePath = "input.txt";

// Puedes crear el archivo 'input.txt' con este contenido de ejemplo para probar:
File.WriteAllText(inputFilePath, @"
    Hola, mi correo es test@ejemplo.com.
    También puedes contactarme en otro.correo@mi-dominio.es.
    Y mi amigo usa Usuario123@Sub.Dominio.ORG.
    Esto no es un correo: no-correo.com
    Ni esto: @dominio.com
    Pero este sí: anidado.prueba@email.net
    ");

Console.WriteLine($"Extrayendo correos de '{inputFilePath}'...");
// Ahora puedes llamar al método estático de EmailExtractor directamente.
List<string> foundEmails = EmailExtractor.ExtractEmailsFromFile(inputFilePath);

if (foundEmails.Any())
{
    Console.WriteLine("\nCorreos encontrados (únicos y ordenados):");
    foreach (string email in foundEmails)
    {
        Console.WriteLine(email);
    }
}
else
{
    Console.WriteLine("\nNo se encontraron correos o hubo un error.");
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey();