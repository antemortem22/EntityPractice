using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ventas_mensuales.Entities;

namespace Ventas_mensuales
{
    internal class DBcontext
    {

        public class VentasMensualesDbContext : DbContext
        {
            
            public VentasMensualesDbContext(DbContextOptions<VentasMensualesDbContext> options) : base(options)
            {

            }

            public DbSet<VentasMensuales> VentasMensuales { get; set; }
            public DbSet<Rechazos> Rechazos { get; set; }
            public DbSet<Parametria> Parametria { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //Config Entity to table "VentasMensuales"
                modelBuilder.Entity<VentasMensuales>(entity =>
                {
                    entity.ToTable("ventas_mensuales");

                    entity.Property(vm => vm.IdVendedor)
                        .HasColumnName("id_vendedor");
                    entity.Property(vm => vm.FechaInforme)
                        .HasColumnName("fecha_informe");
                    entity.Property(vm => vm.Venta)
                        .HasColumnName("venta");
                    entity.Property(vm => vm.EmpresaGrande)
                        .HasColumnName("empresa_grande");
                });

                // Config Entity to table "Rechazos"
                modelBuilder.Entity<Rechazos>(entity =>
                {
                    entity.ToTable("rechazos");

                    entity.Property(r => r.IdLineaRechazada)
                        .HasColumnName("id_linea_rechazada");

                    entity.Property(r => r.Motivo)
                        .HasColumnName("motivo");
                });

                // Config Entity to table "Parametria"
                modelBuilder.Entity<Parametria>().HasNoKey();
               

                base.OnModelCreating(modelBuilder);

            }

        }
    }

}

