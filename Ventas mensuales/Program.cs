using Microsoft.EntityFrameworkCore;
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

            
            
         

        }
    }
}

