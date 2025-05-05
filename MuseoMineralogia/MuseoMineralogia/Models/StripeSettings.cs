using System;

namespace MuseoMineralogia.Models
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }

    // Classe per tracciare i dati della sessione Stripe
    public class OrdineStripe
    {
        public string SessionId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}