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
                //Config Entity to the table "VentasMensuales"
                modelBuilder.Entity<VentasMensuales>(entity =>
                {
                    entity.ToTable("ventas_mensuales");

                    entity.HasKey(vm => new { vm.IdVendedor, vm.FechaInforme });

                    entity.Property(vm => vm.IdVendedor).HasMaxLength(3).IsRequired();
                    entity.Property(vm => vm.FechaInforme).IsRequired();
                    entity.Property(vm => vm.Venta).HasColumnType("decimal(8,2)").IsRequired();
                    entity.Property(vm => vm.EmpresaGrande).HasMaxLength(1).IsRequired();
                });

                // Config Entity to the table "Rechazos"
                modelBuilder.Entity<Rechazos>(entity =>
                {
                    entity.ToTable("rechazos");

                    entity.HasKey(r => r.IdRechazo);

                    entity.Property(r => r.Motivo).HasMaxLength(100).IsRequired();
                });

                // Config Entity to the table "Parametria"
                modelBuilder.Entity<Parametria>(entity =>
                {
                    entity.ToTable("parametria");

                    entity.HasKey(p => p.Fecha);

                    entity.Property(p => p.Fecha).IsRequired();
                });

                base.OnModelCreating(modelBuilder);

            }

        }
    }

}

