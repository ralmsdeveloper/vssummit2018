using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;

namespace NetFramework461
{
    class Program
    {
        private static readonly int[] _opcoesInsercao = new[] { 1, 10, 100, 1000 };
        private static readonly int _for = 1;
        private static IEnumerable<Evento> _eventos;

        static void Main(string[] args)
        {

            for (int i = 1; i <= _for; i++)
            {

                Executar(
                    EF6:
                    () =>
                    {
                        using (var ef6Cxt = new EF6Context())
                        {
                            TestarEF6(ef6Cxt);
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

        private static void Executar(Action EF6, Action EFCore)
        {

            EF6();

            Console.WriteLine("-------------------------------------------------");

            EFCore();

            Console.WriteLine("\n\n");
        }

        private static void TestarEFCore(EventoContext ctx)
        {
            var tempo = new Stopwatch();

            // Apagar e Criar o banco de dados
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            // Insert
            foreach (var item in _opcoesInsercao)
            {
                _eventos = GetEventos(item);

                tempo.Restart();
                ctx.AddRange(_eventos);
                ctx.SaveChanges();
                tempo.Stop();
                Console.WriteLine($"Tempo Insert EF Core  {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }

            // Select
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            foreach (var item in _opcoesInsercao)
            {
                tempo.Restart();
                _eventos = ctx.Set<Evento>().Take(item).ToList();
                tempo.Stop();
                Console.WriteLine($"Tempo Select EF Core  {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }
        }

        private static void TestarEF6(EF6Context ctx)
        {
            var tempo = new Stopwatch();

            // Apagar e Criar o banco de dados
            using (var db = new EventoContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            // Insert
            foreach (var item in _opcoesInsercao)
            {
                _eventos = GetEventos(item);
                
                tempo.Restart();
                ctx.Set<Evento>().AddRange(_eventos);
                ctx.SaveChanges();
                tempo.Stop();
                Console.WriteLine($"Tempo Insert EF6     {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }

            // Select
            foreach (var item in _opcoesInsercao)
            {
                tempo.Restart();
                _eventos = ctx.Set<Evento>().Take(item).ToList();
                tempo.Stop();
                Console.WriteLine($"Tempo Select EF6   {item.ToString().PadLeft(5, ' ')} Registro(s): {tempo.Elapsed}");
            }
        }

        private static IEnumerable<Evento> GetEventos(int total)
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
    }
}
