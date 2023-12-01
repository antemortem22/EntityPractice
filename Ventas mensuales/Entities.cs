using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas_mensuales
{
    internal class Entities
    {

        public class VentasMensuales
        {
            public string IdVendedor { get; set; }
            public DateTime FechaInforme { get; set; }
            public decimal Venta { get; set; }
            public string EmpresaGrande { get; set; }
        }

        public class Rechazos
        {
            public string IdLineaRechazada { get; set; }
            public string Motivo { get; set; } 
        }

        public class Parametria
        {
            public DateTime FechaProceso { get; set; }
        }
    }
}
