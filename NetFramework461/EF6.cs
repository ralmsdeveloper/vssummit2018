using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramework461
{
    public partial class EF6Context : DbContext
    {
        public EF6Context()
            : base("name=DevPrimeOneEF21")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Palestra>().ToTable("Palestras");

            modelBuilder
                .Entity<Evento>()
                .ToTable("Eventos");
        }
    }
}
