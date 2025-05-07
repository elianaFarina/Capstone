using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Data;
using MuseoMineralogia.Models;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MuseoMineralogia.Controllers
{
    public class BiglietteriaController : Controller
    {
        private readonly MuseoContext _context;
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<BiglietteriaController> _logger;

        public BiglietteriaController(
            MuseoContext context,
            IOptions<StripeSettings> stripeSettings,
            UserManager<Utente> userManager,
            ILogger<BiglietteriaController> logger)
        {
            _context = context;
            _stripeSettings = stripeSettings;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Biglietteria
        public async Task<IActionResult> Index()
        {
            try
            {
                var tipiBiglietto = await _context.TipiBiglietto.ToListAsync();
                return View(tipiBiglietto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei tipi di biglietto");
                return View(new List<TipoBiglietto>());
            }
        }

        // GET: Biglietteria/Dettaglio/5
        public async Task<IActionResult> Dettaglio(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoBiglietto = await _context.TipiBiglietto
                .FirstOrDefaultAsync(m => m.TipoBigliettoId == id);
            if (tipoBiglietto == null)
            {
                return NotFound();
            }

            return View(tipoBiglietto);
        }

        // GET: Biglietteria/Acquista/5
        [Authorize]
        public async Task<IActionResult> Acquista(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoBiglietto = await _context.TipiBiglietto
                .FirstOrDefaultAsync(m => m.TipoBigliettoId == id);
            if (tipoBiglietto == null)
            {
                return NotFound();
            }

            ViewBag.StripePublishableKey = _stripeSettings.Value.PublishableKey;
            return View(tipoBiglietto);
        }

        // POST: Biglietteria/AggiungiAlCarrello
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AggiungiAlCarrello(int bigliettoId, int quantita)
        {
            // Verifica che i dati siano validi
            if (quantita <= 0)
            {
                TempData["ErrorMessage"] = "La quantità deve essere maggiore di zero";
                return RedirectToAction("Index");
            }

            // Ottieni l'utente corrente
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Ottieni o crea il carrello dell'utente
            var carrello = await _context.Carrelli
                .FirstOrDefaultAsync(c => c.UtenteId == userId);

            if (carrello == null)
            {
                carrello = new Carrello
                {
                    UtenteId = userId!,
                    ElementiCarrello = new List<ElementoCarrello>()
                };
                _context.Carrelli.Add(carrello);
                await _context.SaveChangesAsync();
            }

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
            return RedirectToAction("Index", "Carrello");
        }

        // POST: Biglietteria/CreaPagamento
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreaPagamento([FromBody] AcquistoModel model)
        {
            try
            {
                // Validazione input
                if (model == null || model.BigliettoId <= 0 || model.Quantita <= 0)
                {
                    return BadRequest(new { success = false, message = "Parametri non validi" });
                }

                // Ottieni l'utente corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Utente non autenticato" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "Utente non trovato" });
                }

                // Ottieni il biglietto
                var biglietto = await _context.TipiBiglietto.FindAsync(model.BigliettoId);
                if (biglietto == null)
                {
                    return NotFound(new { success = false, message = "Tipo di biglietto non trovato" });
                }

                // Crea una sessione Stripe Checkout
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(biglietto.Prezzo * 100), // Converti in centesimi
                                Currency = "eur",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = biglietto.Nome ?? "Biglietto",
                                    Description = "Biglietto Museo Mineralogia"
                                }
                            },
                            Quantity = model.Quantita
                        }
                    },
                    Mode = "payment",
                    CustomerEmail = user.Email,
                    SuccessUrl = Url.Action("Success", "Biglietteria", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = Url.Action("Cancel", "Biglietteria", null, Request.Scheme)
                };

                // Crea la sessione di checkout
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
                var dettaglio = new DettaglioOrdine
                {
                    OrdineId = ordine.OrdineId,
                    TipoBigliettoId = model.BigliettoId,
                    Quantita = model.Quantita,
                    PrezzoUnitario = biglietto.Prezzo
                };

                _context.DettagliOrdine.Add(dettaglio);
                await _context.SaveChangesAsync();

                // Ritorna il Session ID per il redirect a Stripe Checkout
                return Json(new { success = true, sessionId = session.Id });
            }
            catch (StripeException se)
            {
                _logger.LogError(se, "Errore Stripe durante la creazione del pagamento");
                return StatusCode(500, new { success = false, message = "Errore durante la creazione del pagamento", error = se.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del pagamento");
                return StatusCode(500, new { success = false, message = "Errore durante la creazione del pagamento" });
            }
        }

        // GET: Biglietteria/Success
        [Authorize]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Verifica lo stato della sessione
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(session_id);

                // Trova l'ordine associato
                var ordine = await _context.Ordini
                    .FirstOrDefaultAsync(o => o.SessionId == session_id);

                if (ordine == null)
                {
                    ViewBag.ErrorMessage = "Ordine non trovato";
                    return View("Error");
                }

                // Carica separatamente i dettagli dell'ordine con i tipi di biglietto
                var dettagli = await _context.DettagliOrdine
                    .Where(d => d.OrdineId == ordine.OrdineId)
                    .ToListAsync();

                // Carica i tipi di biglietto per ogni dettaglio
                foreach (var dettaglio in dettagli)
                {
                    dettaglio.TipoBiglietto = await _context.TipiBiglietto
                        .FindAsync(dettaglio.TipoBigliettoId);
                }

                ordine.DettagliOrdine = dettagli;

                // Aggiorna lo stato dell'ordine in base allo stato del pagamento
                if (session.PaymentStatus == "paid")
                {
                    ordine.Stato = "Pagato";
                    await _context.SaveChangesAsync();
                }
                else if (session.PaymentStatus == "unpaid")
                {
                    ordine.Stato = "Non pagato";
                    await _context.SaveChangesAsync();
                }

                ViewBag.SessionId = session_id;
                ViewBag.PaymentStatus = session.PaymentStatus;

                return View(ordine);
            }
            catch (StripeException se)
            {
                _logger.LogError(se, "Errore Stripe durante la verifica del pagamento");
                ViewBag.ErrorMessage = "Errore durante la verifica del pagamento";
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica del pagamento");
                ViewBag.ErrorMessage = "Errore durante la verifica del pagamento";
                return View("Error");
            }
        }

        // GET: Biglietteria/Cancel
        public IActionResult Cancel()
        {
            return View();
        }

        // GET: Biglietteria/MieiOrdini
        [Authorize]
        public async Task<IActionResult> MieiOrdini()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ordini = await _context.Ordini
                .Where(o => o.UtenteId == userId)
                .OrderByDescending(o => o.DataOrdine)
                .ToListAsync();

            // Carica manualmente i dettagli ordine e i tipi di biglietto
            foreach (var ordine in ordini)
            {
                // Carica i dettagli ordine
                var dettagli = await _context.DettagliOrdine
                    .Where(d => d.OrdineId == ordine.OrdineId)
                    .ToListAsync();

                // Carica i tipi di biglietto per ogni dettaglio
                foreach (var dettaglio in dettagli)
                {
                    dettaglio.TipoBiglietto = await _context.TipiBiglietto
                        .FindAsync(dettaglio.TipoBigliettoId);
                }

                ordine.DettagliOrdine = dettagli;
            }

            return View(ordini);
        }

        // GET: Biglietteria/DettaglioOrdine/5
        [Authorize]
        public async Task<IActionResult> DettaglioOrdine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ordine = await _context.Ordini
                .FirstOrDefaultAsync(o => o.OrdineId == id && o.UtenteId == userId);

            if (ordine == null)
            {
                return NotFound();
            }

            // Carica manualmente i dettagli ordine
            var dettagli = await _context.DettagliOrdine
                .Where(d => d.OrdineId == ordine.OrdineId)
                .ToListAsync();

            // Carica i tipi di biglietto per ogni dettaglio
            foreach (var dettaglio in dettagli)
            {
                dettaglio.TipoBiglietto = await _context.TipiBiglietto
                    .FindAsync(dettaglio.TipoBigliettoId);
            }

            ordine.DettagliOrdine = dettagli;

            return View(ordine);
        }

        // POST: Biglietteria/CancellaOrdine/5
        [Authorize]
        [HttpPost, ActionName("CancellaOrdine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancellaOrdineConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Carica l'ordine senza include
            var ordine = await _context.Ordini
                .FirstOrDefaultAsync(o => o.OrdineId == id && o.UtenteId == userId);

            if (ordine == null)
            {
                return NotFound();
            }

            // Puoi annullare l'ordine solo se è in attesa di pagamento
            if (ordine.Stato != "In attesa di pagamento")
            {
                TempData["ErrorMessage"] = "Non è possibile annullare un ordine che è già stato pagato";
                return RedirectToAction(nameof(MieiOrdini));
            }

            // Carica separatamente i dettagli dell'ordine
            var dettagliOrdine = await _context.DettagliOrdine
                .Where(d => d.OrdineId == ordine.OrdineId)
                .ToListAsync();

            // Rimuovi i dettagli dell'ordine
            if (dettagliOrdine.Any())
            {
                _context.DettagliOrdine.RemoveRange(dettagliOrdine);
            }

            // Rimuovi l'ordine
            _context.Ordini.Remove(ordine);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ordine annullato con successo";
            return RedirectToAction(nameof(MieiOrdini));
        }
    }
}