using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Data;
using MuseoMineralogia.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MuseoMineralogia.Controllers
{
    [Authorize]
    public class CarrelloController : Controller
    {
        private readonly MuseoContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<CarrelloController> _logger;
        private readonly IConfiguration _configuration;

        public CarrelloController(
        MuseoContext context,
        UserManager<Utente> userManager,
        ILogger<CarrelloController> logger,
        IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }
        // GET: Carrello
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrello = await OttieniOCreaCarrello(userId);

            // Carica gli elementi del carrello e i relativi biglietti
            await _context.Entry(carrello)
                .Collection(c => c.ElementiCarrello!)
                .LoadAsync();

            if (carrello.ElementiCarrello != null)
            {
                foreach (var elemento in carrello.ElementiCarrello)
                {
                    await _context.Entry(elemento)
                        .Reference(e => e.TipoBiglietto!)
                        .LoadAsync();
                }
            }

            return View(carrello);
        }

        // POST: Carrello/Aggiungi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aggiungi(int bigliettoId, int quantita)
        {
            if (quantita <= 0)
                return BadRequest("La quantità deve essere maggiore di zero");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrello = await OttieniOCreaCarrello(userId);

            // Verifica se l'elemento esiste già nel carrello
            var elementoEsistente = await _context.ElementiCarrello
                .FirstOrDefaultAsync(e => e.CarrelloId == carrello.CarrelloId && e.TipoBigliettoId == bigliettoId);

            if (elementoEsistente != null)
            {
                // Aggiorna la quantità
                elementoEsistente.Quantita += quantita;
            }
            else
            {
                // Aggiungi nuovo elemento
                var nuovoElemento = new ElementoCarrello
                {
                    CarrelloId = carrello.CarrelloId,
                    TipoBigliettoId = bigliettoId,
                    Quantita = quantita
                };

                _context.ElementiCarrello.Add(nuovoElemento);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Biglietto aggiunto al carrello";
            return RedirectToAction(nameof(Index));
        }

        // POST: Carrello/Rimuovi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rimuovi(int elementoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrello = await OttieniOCreaCarrello(userId);

            var elemento = await _context.ElementiCarrello
                .FirstOrDefaultAsync(e => e.ElementoCarrelloId == elementoId && e.CarrelloId == carrello.CarrelloId);

            if (elemento != null)
            {
                _context.ElementiCarrello.Remove(elemento);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Elemento rimosso dal carrello";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Carrello/SvuotaCarrello
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SvuotaCarrello()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrello = await OttieniOCreaCarrello(userId);

            var elementiCarrello = await _context.ElementiCarrello
                .Where(e => e.CarrelloId == carrello.CarrelloId)
                .ToListAsync();

            if (elementiCarrello.Any())
            {
                _context.ElementiCarrello.RemoveRange(elementiCarrello);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Carrello svuotato";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Carrello/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrello = await OttieniOCreaCarrello(userId);

            // Carica gli elementi del carrello e i relativi biglietti
            await _context.Entry(carrello)
                .Collection(c => c.ElementiCarrello!)
                .LoadAsync();

            if (carrello.ElementiCarrello == null || !carrello.ElementiCarrello.Any())
            {
                TempData["ErrorMessage"] = "Il carrello è vuoto";
                return RedirectToAction(nameof(Index));
            }

            foreach (var elemento in carrello.ElementiCarrello)
            {
                await _context.Entry(elemento)
                    .Reference(e => e.TipoBiglietto!)
                    .LoadAsync();
            }
            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];

            return View(carrello);
        }

        // POST: Carrello/CreaPagamento
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreaPagamento()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "Utente non autenticato" });
                }

                var carrello = await OttieniOCreaCarrello(userId);

                // Carica gli elementi del carrello e i relativi biglietti
                await _context.Entry(carrello)
                    .Collection(c => c.ElementiCarrello!)
                    .LoadAsync();

                if (carrello.ElementiCarrello == null || !carrello.ElementiCarrello.Any())
                {
                    return BadRequest(new { success = false, message = "Il carrello è vuoto" });
                }

                // Carica i dettagli dei biglietti
                foreach (var elemento in carrello.ElementiCarrello)
                {
                    await _context.Entry(elemento)
                        .Reference(e => e.TipoBiglietto!)
                        .LoadAsync();
                }

                // Crea elementi per Stripe
                var lineItems = new List<SessionLineItemOptions>();

                foreach (var elemento in carrello.ElementiCarrello)
                {
                    if (elemento.TipoBiglietto != null)
                    {
                        lineItems.Add(new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(elemento.TipoBiglietto.Prezzo * 100), // Converti in centesimi
                                Currency = "eur",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = elemento.TipoBiglietto.Nome ?? "Biglietto",
                                    Description = "Biglietto Museo Mineralogia"
                                }
                            },
                            Quantity = elemento.Quantita
                        });
                    }
                }

                // Crea la sessione Stripe
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    CustomerEmail = user.Email,
                    SuccessUrl = Url.Action("Success", "Biglietteria", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = Url.Action("Cancel", "Biglietteria", null, Request.Scheme)
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                // Salva informazioni dell'ordine nel database
                var ordine = new Ordine
                {
                    UtenteId = userId,
                    DataOrdine = DateTime.Now,
                    Stato = "In attesa di pagamento",
                    SessionId = session.Id,
                    PaymentIntentId = session.PaymentIntentId
                };

                _context.Ordini.Add(ordine);
                await _context.SaveChangesAsync();

                // Aggiungi dettagli ordine
                foreach (var elemento in carrello.ElementiCarrello)
                {
                    if (elemento.TipoBiglietto != null)
                    {
                        var dettaglio = new DettaglioOrdine
                        {
                            OrdineId = ordine.OrdineId,
                            TipoBigliettoId = elemento.TipoBigliettoId,
                            Quantita = elemento.Quantita,
                            PrezzoUnitario = elemento.TipoBiglietto.Prezzo
                        };

                        _context.DettagliOrdine.Add(dettaglio);
                    }
                }

                await _context.SaveChangesAsync();

                // Svuota il carrello dopo aver creato l'ordine
                var elementiCarrello = await _context.ElementiCarrello
                    .Where(e => e.CarrelloId == carrello.CarrelloId)
                    .ToListAsync();

                _context.ElementiCarrello.RemoveRange(elementiCarrello);
                await _context.SaveChangesAsync();

                // Ritorna il Session ID per il redirect a Stripe Checkout
                return Json(new { success = true, sessionId = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del pagamento");
                return StatusCode(500, new { success = false, message = "Errore durante la creazione del pagamento" });
            }
        }

        // Metodo helper per ottenere o creare un carrello
        private async Task<Carrello> OttieniOCreaCarrello(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var carrello = await _context.Carrelli
                .FirstOrDefaultAsync(c => c.UtenteId == userId);

            if (carrello == null)
            {
                carrello = new Carrello
                {
                    UtenteId = userId,
                    ElementiCarrello = new List<ElementoCarrello>()
                };

                _context.Carrelli.Add(carrello);
                await _context.SaveChangesAsync();
            }

            return carrello;
        }
    }
}