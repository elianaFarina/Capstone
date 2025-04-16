using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Models;

namespace MuseoMineralogia.Data
{
    public class MuseoContext : IdentityDbContext<Utente>
    {
        public MuseoContext(DbContextOptions<MuseoContext> options) : base(options) { }

        public DbSet<TipoBiglietto> TipiBiglietto { get; set; } = null!;
        public DbSet<Ordine> Ordini { get; set; } = null!;
        public DbSet<DettaglioOrdine> DettagliOrdine { get; set; } = null!;
        public DbSet<Carrello> Carrelli { get; set; } = null!;
        public DbSet<ElementoCarrello> ElementiCarrello { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TipoBiglietto>(entity =>
            {
                entity.HasKey(e => e.TipoBigliettoId);
                entity.Property(e => e.Prezzo).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Carrello>(entity =>
            {
                entity.HasKey(e => e.CarrelloId);
                entity.HasOne(c => c.Utente)
                    .WithOne(u => u.Carrello)
                    .HasForeignKey<Carrello>(c => c.UtenteId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.ElementiCarrello)
                    .WithOne(ec => ec.Carrello)
                    .HasForeignKey(ec => ec.CarrelloId);
            });

            modelBuilder.Entity<Ordine>(entity =>
            {
                entity.HasKey(e => e.OrdineId);
                entity.HasOne(o => o.Utente)
                    .WithMany(u => u.Ordini)
                    .HasForeignKey(o => o.UtenteId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.DettagliOrdine)
                    .WithOne(d => d.Ordine)
                    .HasForeignKey(d => d.OrdineId);
            });
            modelBuilder.Entity<DettaglioOrdine>(entity =>
            {
                entity.HasKey(e => e.DettaglioOrdineId);
                entity.Property(e => e.PrezzoUnitario)
                      .HasPrecision(10, 2);
            });
        }
    }
}