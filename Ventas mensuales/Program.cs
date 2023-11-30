using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using static Ventas_mensuales.DBcontext;


namespace Ventas_mensuales
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Path to txt file
            List<string> lineas = File.ReadAllLines(@"C:\Users\Antemortem\Desktop\Ventas mensuales\Ventas mensuales\bin\Debug\net6.0\data.txt").ToList();
            //Connection to the db using a path
            string stringConnection = @"Data Source=localhost;Initial Catalog=VentasMensuales;Integrated Security=True;Trust Server Certificate=True";
            var options = new DbContextOptionsBuilder<VentasMensualesDbContext>().UseSqlServer(stringConnection).Options;
            var context = new VentasMensualesDbContext(options);

            //Exercise 
            foreach (string line in lineas)
            {
                ParseLine(line);
            }
        }

        static void ParseLine(string line)
        {
            
            string fechaStr = line.Substring(0, 8);
            string id = line.Substring(8, 3);
            string venta = line.Substring(11, 11);
            string letra = line.Substring(22, 1);

            string fechaFormateada = $"{fechaStr.Substring(0, 4)}-{fechaStr.Substring(4, 2)}-{fechaStr.Substring(6, 2)}";

            Console.WriteLine($"Fecha: {fechaFormateada}, ID: {id}, Monto: {venta}, Letra: {letra}");

        }





    }

}

