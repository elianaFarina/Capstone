using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Models;

namespace MuseoMineralogia.Data
{
    public class MuseoContext : DbContext
    {
        public MuseoContext(DbContextOptions<MuseoContext> options) : base(options) { }

        public DbSet<Utente> Utenti { get; set; }
        public DbSet<TipoBiglietto> TipiBiglietto { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordine>()
                .Property(o => o.Totale)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TipoBiglietto>()
                .Property(t => t.Prezzo)
                .HasPrecision(18, 2);
        }
    }
}
