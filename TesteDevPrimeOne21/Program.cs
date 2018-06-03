using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Bulk;
using System.Data.SqlClient;
using System.Linq;
using Dapper.Contrib.Extensions;
using System.Diagnostics;

namespace TesteDevPrimeOne21
{
    class Program
    {
        private static readonly int[] _opcoesInsercao = new[] { 1, 10, 100, 1000 };
        private static readonly int _for = 2;

        static void Main(string[] args)
        {
            for (int i = 1; i <= _for; i++)
            {

                Executar(
                    Dapper:
                    () =>
                    {
                        using (var dapper = new EventoContext())
                        {
                            TestarDapper(dapper);
                        }
                    },
                    EFCore:
                    () =>
                    {
                        using (var efcore = new EventoContext())
                        {
                            TestarEFCore(efcore);
                        }
                    });
            }
            Console.ReadKey();
        }

        private static void TestarEFCore(EventoContext ctx)
        {
            var tempo = new Stopwatch();

            CriarBanco(ctx);

            // ctx.ChangeTracker.AutoDetectChangesEnabled = false;
            foreach (var item in _opcoesInsercao)
            {
                var eventos = GetEventos(item, item).ToList();
                tempo.Restart();
                ctx.AddRange(eventos);
                ctx.SaveChanges();
                tempo.Stop();
                Console.WriteLine($"Tempo Insert EF Core  {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }

            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            foreach (var item in _opcoesInsercao)
            {
                tempo.Restart();
                var eventos = ctx.Set<Evento>().Take(item).ToList();
                tempo.Stop();
                Console.WriteLine($"Tempo Select EF Core  {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }
        }

        private static void TestarDapper(EventoContext ctx)
        {
           var tempo = new Stopwatch();

            CriarBanco(ctx);

            var dapper = (SqlConnection)ctx.Database.GetDbConnection();
            if (dapper.State != System.Data.ConnectionState.Open)
            {
                dapper.Open();
            }

            foreach (var item in _opcoesInsercao)
            {
                var eventos = GetEventos(item, item);
                tempo.Restart();
                dapper.Insert(eventos);
                //dapper.BulkInsert(eventos);
                tempo.Stop();
                Console.WriteLine($"Tempo Insert Dapper   {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }

            foreach (var item in _opcoesInsercao)
            {
                tempo.Restart();
                var eventos = dapper.Query<Evento>($"select top {item} * from Eventos").ToList();
                tempo.Stop();
                Console.WriteLine($"Tempo Select Dapper   {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }
        }

        private static void Executar(Action Dapper, Action EFCore)
        {

            Dapper();

            Console.WriteLine("-------------------------------------------------");

            EFCore();

            Console.WriteLine("\n\n");
        }

        private static IEnumerable<Evento> GetEventos(int total, int item = 0)
        {
           for (int i = 0; i < total; i++)
            {
                yield return new Evento
                {
                    Data = DateTime.Now,
                    Descricao = $"Evento Teste {i}",
                    Pessoas = i
                };
            }
        }

        private static void CriarBanco(EventoContext ctx)
        {
            if (!ctx.Database.EnsureCreated())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Eventos");
                ctx.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Eventos', RESEED, 0)");
            }
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
                // MaxBatchSize: Tamanho do Lote a ser enviar para o Servidor!
                optionsBuilder.UseSqlServer(Conexao, p=>p.MaxBatchSize(10000));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>(
                p =>
                {
                    p.HasKey(x => x.Id);
                    p.Property(x => x.Id).UseSqlServerIdentityColumn();
                    p.Property(x => x.Descricao).HasMaxLength(100);
                    p.ToTable("Eventos");
                });
        }
    }
}
