using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Ventas_mensuales.DBcontext;
using static Ventas_mensuales.Entities;


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
                ParseLine(line, context);
            }

            ListarVendedoresSuperaron100000(context);
            ListarVendedoresNoSuperaron100000(context);
            ListarVendedoresVendieronAEmpresaGrande(context);
            ListarRechazos(context);

        }

        static void ParseLine(string line, VentasMensualesDbContext context)
        {
            string fecha = line.Substring(0, 8);
            string id = line.Substring(8, 3);
            string venta = line.Substring(11, 11);
            string letra = line.Substring(22, 1);


            string fecha2 = $"{fecha.Substring(0, 4)}-{fecha.Substring(4, 2)}-{fecha.Substring(6, 2)}";
            DateTime fechaFormateada = DateTime.ParseExact(fecha, "yyyyMMdd", CultureInfo.InvariantCulture);

            decimal ventaDecimal = decimal.Parse(venta) / 100;

            Console.WriteLine($"Fecha: {fechaFormateada}, ID: {id}, Monto: {ventaDecimal}, Letra: {letra}");
            if (string.IsNullOrWhiteSpace(id) && id == "   ")
            {
                // Registro rechazado: Falta código de vendedor
                InsertarRegistroRechazo(context, line, "Falta código de vendedor");
                return;
            }



            if (letra != "S" && letra != "N")
            {
                // Registro rechazado: El flag "Venta a empresa grande" tiene un valor incorrecto
                InsertarRegistroRechazo(context, line, "El flag \"Venta a empresa grande\" tiene un valor incorrecto");
                return;
            }

            Parametria instanciaParametria = context.Parametria.FirstOrDefault()!;
            //if (fechaFormateada != instanciaParametria) 
            //{ }

            // Registro válido: Insertar en ventas_mensuales
            InsertarVentaMensual(context, fechaFormateada, id, ventaDecimal, letra);







        }


        static void InsertarRegistroRechazo(VentasMensualesDbContext context, string linea, string motivoRechazo)
        {
            // Insertar el registro rechazado en la tabla de rechazos
            Rechazos rechazo = new Rechazos
            {
                IdLineaRechazada = linea,
                Motivo = motivoRechazo
            };

            context.Rechazos.Add(rechazo);
            context.SaveChanges();
        }
        static void InsertarVentaMensual(VentasMensualesDbContext context, DateTime fechaFormateada, string id, decimal venta, string letra)
        {
            // Insertar el registros que pasarob el filtro en la tabla de ventas_mesuales
            VentasMensuales ventaMensual = new VentasMensuales
            {
                FechaInforme = fechaFormateada,
                IdVendedor = id,
                Venta = venta,
                EmpresaGrande = letra
            };

            context.VentasMensuales.Add(ventaMensual);
            context.SaveChanges();
        }

        static void ListarVendedoresSuperaron100000(VentasMensualesDbContext context)
        {
            var vendedoresSuperaron100000 = context.VentasMensuales
                .Where(venta => venta.Venta > 100000)
                .Select(venta => $"El vendedor {venta.IdVendedor} vendió {venta.Venta}")
                .ToList();

            Console.WriteLine("Vendedores que superaron los 100.000:");
            foreach (var mensaje in vendedoresSuperaron100000)
            {
                Console.WriteLine(mensaje);
            }
        }

        static void ListarVendedoresNoSuperaron100000(VentasMensualesDbContext context)
        {
            var vendedoresNoSuperaron100000 = context.VentasMensuales
                .Where(venta => venta.Venta <= 100000)
                .Select(venta => $"El vendedor {venta.IdVendedor} vendió {venta.Venta}")
                .ToList();

            Console.WriteLine("Vendedores que no superaron los 100.000:");
            foreach (var mensaje in vendedoresNoSuperaron100000)
            {
                Console.WriteLine(mensaje);
            }
        }

        static void ListarVendedoresVendieronAEmpresaGrande(VentasMensualesDbContext context)
        {
            var vendedoresAEmpresaGrande = context.VentasMensuales
                .Where(venta => venta.EmpresaGrande == "S")
                .Select(venta => venta.IdVendedor)
                .Distinct()
                .ToList();

            Console.WriteLine("Vendedores que vendieron al menos una vez a una empresa grande:");
            foreach (var codigoVendedor in vendedoresAEmpresaGrande)
            {
                Console.WriteLine(codigoVendedor);
            }
        }

        static void ListarRechazos(VentasMensualesDbContext context)
        {
            var registrosRechazados = context.Rechazos
                .Select(rechazo => $"{rechazo.Motivo}: {rechazo.IdLineaRechazada}")
                .ToList();

            Console.WriteLine("Registros rechazados:");
            foreach (var mensaje in registrosRechazados)
            {
                Console.WriteLine(mensaje);
            }
        }
    }
        


  

}

