using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Models;

namespace MuseoMineralogia.Data
{
    public class MuseoMineralogiaDb : DbContext
    {
        public MuseoMineralogiaDb (DbContextOptions<MuseoContext> options) : base(options) { }

        public DbSet<Utente> Utenti { get; set; }
        public DbSet<TipoBiglietto> TipiBiglietto { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<DettaglioOrdine> DettagliOrdine { get; set; }
        public DbSet<Carrello> Carrelli { get; set; }
        public DbSet<ElementoCarrello> ElementiCarrello { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurazione delle relazioni

            // Seed dei tipi di biglietto
            modelBuilder.Entity<TipoBiglietto>().HasData(
                new TipoBiglietto { TipoBigliettoId = 1, Nome = "Intero", Prezzo = 10.00m },
                new TipoBiglietto { TipoBigliettoId = 2, Nome = "Ridotto (fino a 15 anni)", Prezzo = 8.00m }
            );

            modelBuilder.Entity<Ordine>()
                .HasOne(o => o.Utente) // Un ordine ha un utente
                .WithMany(u => u.Ordini) // Un utente può avere molti ordini
                .HasForeignKey(o => o.UtenteId); // Chiave esterna

            modelBuilder.Entity<DettaglioOrdine>()
                .HasOne(d => d.Ordine)
                .WithMany(o => o.DettagliOrdine)
                .HasForeignKey(d => d.OrdineId);

            modelBuilder.Entity<DettaglioOrdine>()
                .HasOne(d => d.TipoBiglietto)
                .WithMany()
                .HasForeignKey(d => d.TipoBigliettoId);

            modelBuilder.Entity<Carrello>()
                .HasOne(c => c.Utente)
                .WithOne(u => u.Carrello)
                .HasForeignKey<Carrello>(c => c.UtenteId); 

            modelBuilder.Entity<ElementoCarrello>()
                .HasOne(e => e.Carrello)
                .WithMany(c => c.ElementiCarrello)
                .HasForeignKey(e => e.CarrelloId);

            modelBuilder.Entity<ElementoCarrello>()
                .HasOne(e => e.TipoBiglietto)
                .WithMany()
                .HasForeignKey(e => e.TipoBigliettoId);

            modelBuilder.Entity<DettaglioOrdine>()
                .Property(d => d.PrezzoUnitario)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<TipoBiglietto>()
                .Property(t => t.Prezzo)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
