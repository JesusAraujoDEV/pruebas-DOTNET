// EmailExtractor.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions; // Necesario para expresiones regulares

// Define un namespace para tu lógica de extracción de correos.
// Es común usar el nombre del proyecto como base.
namespace ExtractorConsoleApp.Utilities // Puedes usar solo 'MiProyecto' o algo más específico como 'MiProyecto.Extractors'
{
    public class EmailExtractor
    {
        // Método principal para ejecutar el extractor
        public static List<string> ExtractEmailsFromFile(string filePath)
        {
            HashSet<string> uniqueEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string fileContent = File.ReadAllText(filePath);
                Regex emailRegex = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase);
                MatchCollection matches = emailRegex.Matches(fileContent);

                foreach (Match match in matches)
                {
                    uniqueEmails.Add(match.Value);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: El archivo no fue encontrado en la ruta: {filePath}");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error inesperado al leer el archivo o extraer correos: {ex.Message}");
                return new List<string>();
            }

            List<string> result = uniqueEmails.ToList();
            result.Sort();

            return result;
        }
    }
} // Fin del namespace