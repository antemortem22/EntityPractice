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

            ListarVendedoresSuperaronMonto(context);
            ListarVendedoresNoSuperaronMonto(context);
            ListarVendedoresVendieronAEmpresaGrande(context);
            ListarRechazos(context);

        }

        static void ParseLine(string line, VentasMensualesDbContext context)
        {
            string fecha = line.Substring(0, 8).Trim();
            string id = line.Substring(8, 3).Trim();
            string venta = line.Substring(11, 11).Trim();
            string letra = line.Substring(22, 1).Trim();


            string fecha2 = $"{fecha.Substring(0, 4)}-{fecha.Substring(4, 2)}-{fecha.Substring(6, 2)}";
            DateTime fechaFormateada = DateTime.ParseExact(fecha, "yyyyMMdd", CultureInfo.InvariantCulture);

            decimal ventaDecimal = decimal.Parse(venta) / 100;

            Console.WriteLine($"Fecha: {fechaFormateada}, ID: {id}, Monto: {ventaDecimal}, Letra: {letra}");
            if (string.IsNullOrWhiteSpace(id))
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

            DateTime fechaParametria = context.Parametria
                                  .Select(p => p.FechaProceso)
                                  .First();

            
            if (fechaFormateada != fechaParametria) 
            {
                InsertarRegistroRechazo(context, line, "La fecha del mes no corresponde con la fecha paremetrizada");
                return;
            }
           

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

        static void ListarVendedoresSuperaronMonto(VentasMensualesDbContext context)
        {
            var vendedoresSuperaronMonto = context.VentasMensuales
                .Where(venta => venta.Venta > 100000)
                .GroupBy(venta => venta.IdVendedor)
                .Select(group => new
                {
                    IdVendedor = group.Key,
                    TotalVentas = group.Sum(venta => venta.Venta)
                })
                .ToList();

            Console.WriteLine("\n /////////////////////////////////");
            Console.WriteLine("Vendedores que superaron los 100.000 agrupados por IdVendedor:");
            foreach (var resultado in vendedoresSuperaronMonto)
            {
                Console.WriteLine($"El vendedor {resultado.IdVendedor} vendió un total de {resultado.TotalVentas}");
            }
        }

        static void ListarVendedoresNoSuperaronMonto(VentasMensualesDbContext context)
        {
            var vendedoresNoSuperaronMonto = context.VentasMensuales
                .Where(venta => venta.Venta <= 100000)
                .GroupBy(venta => venta.IdVendedor)
                .Select(group => new
                {
                    IdVendedor = group.Key,
                    TotalVentas = group.Sum(venta => venta.Venta)
                })
                .ToList();

            Console.WriteLine("\n /////////////////////////////////");
            Console.WriteLine("Vendedores que no superaron los 100.000:");
            foreach (var resultado in vendedoresNoSuperaronMonto)
            {
                Console.WriteLine($"El vendedor {resultado.IdVendedor} vendió un total de {resultado.TotalVentas}");
            }
        }

        static void ListarVendedoresVendieronAEmpresaGrande(VentasMensualesDbContext context)
        {
            var vendedoresAEmpresaGrande = context.VentasMensuales
                .Where(venta => venta.EmpresaGrande == "S")
                .Select(venta => venta.IdVendedor)
                .Distinct()
                .ToList();

            Console.WriteLine("\n /////////////////////////////////");
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

            Console.WriteLine("\n /////////////////////////////////");
            Console.WriteLine("Registros rechazados:");
            foreach (var mensaje in registrosRechazados)
            {
                Console.WriteLine(mensaje);
            }
        }
    }
        


  

}

