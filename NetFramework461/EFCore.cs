using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramework461
{
    public class EventoContext : DbContext
    {
        private string Conexao => new SqlConnectionStringBuilder
        {
            InitialCatalog = "DevPrimeOneEF21",
            IntegratedSecurity = true,
            DataSource = @"RAFAEL-DELL\SQL2017"
        }.ConnectionString;

        protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Conexao, p => p.MaxBatchSize(10000));
            }
        }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>(
                p =>
                {
                    p.HasKey(x => x.Id);
                    p.Property(x => x.Id).UseSqlServerIdentityColumn();
                    p.Property(x => x.Descricao).HasMaxLength(100);
                    p.ToTable("Eventos");
                });

            modelBuilder.Entity<Palestra>().ToTable("Palestras");
        }
    }
}
