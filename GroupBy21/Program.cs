using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace GroupBy21
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EventoContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var events = db
                    .Set<Evento>()
                    .GroupBy(p => p.Descricao)
                    .Select(p => new { DescricaoY = p.Key, Total = p.Count() })
                    .ToList();
            }

            Console.ReadKey();
        }
    }

    public class EventoContext : DbContext
    {
        private string Conexao => new SqlConnectionStringBuilder
        {
            InitialCatalog = "DevPrimeOneEF21",
            IntegratedSecurity = true,
            DataSource = @"RAFAEL-DELL\SQL2017"
        }.ConnectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Conexao);
                optionsBuilder
                    .UseLoggerFactory(
                        new LoggerFactory()
                        .AddConsole());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>(
                p =>
                {
                    p.Property(x => x.Descricao).HasMaxLength(100);
                    p.ToTable("Eventos");
                });
        }
    }
}
