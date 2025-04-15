using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Models;

namespace MuseoMineralogia.Data
{
    public class MuseoContext : DbContext
    {
        public MuseoContext(DbContextOptions<MuseoContext> options) : base(options) { }

        public DbSet<Utente> Utenti { get; set; } = null!;
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
                entity.Property(e => e.Prezzo)
                      .HasPrecision(10, 2);
            });

            modelBuilder.Entity<DettaglioOrdine>(entity =>
            {
                entity.HasKey(e => e.DettaglioOrdineId);
                entity.Property(e => e.PrezzoUnitario)
                      .HasPrecision(10, 2); 
                entity.HasOne(e => e.Ordine)
                      .WithMany(o => o.DettagliOrdine)
                      .HasForeignKey(e => e.OrdineId);
                entity.HasOne(e => e.TipoBiglietto)
                      .WithMany()
                      .HasForeignKey(e => e.TipoBigliettoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TipoBiglietto>().HasData(
                new TipoBiglietto { TipoBigliettoId = 1, Nome = "Intero", Prezzo = 10.00m },
                new TipoBiglietto { TipoBigliettoId = 2, Nome = "Ridotto (fino a 15 anni)", Prezzo = 8.00m }
            );

            modelBuilder.Entity<Utente>(entity =>
            {
                entity.HasKey(e => e.UtenteId);
                entity.HasOne(e => e.Carrello)
                    .WithOne(c => c.Utente)
                    .HasForeignKey<Carrello>(c => c.UtenteId);
                entity.HasMany(e => e.Ordini)
                    .WithOne(o => o.Utente)
                    .HasForeignKey(o => o.UtenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ordine>(entity =>
            {
                entity.HasKey(e => e.OrdineId);
                entity.HasMany(e => e.DettagliOrdine)
                    .WithOne(d => d.Ordine)
                    .HasForeignKey(d => d.OrdineId);
            });

            modelBuilder.Entity<Carrello>(entity =>
            {
                entity.HasKey(e => e.CarrelloId);
                entity.HasMany(e => e.ElementiCarrello)
                    .WithOne(ec => ec.Carrello)
                    .HasForeignKey(ec => ec.CarrelloId);
            });

            modelBuilder.Entity<ElementoCarrello>(entity =>
            {
                entity.HasKey(e => e.ElementoCarrelloId);
                entity.HasOne(e => e.TipoBiglietto)
                    .WithMany()
                    .HasForeignKey(e => e.TipoBigliettoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}