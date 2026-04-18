namespace Amigo.Domain.Entities;

[Table($"{nameof(Payment)}", Schema = SchemaConstants.payment_schema)]

public class Payment:BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
   
    public string? Note { get; set; }
    public CurrencyCode Currency { get; set; }
    public DateTime PaidAt { get; set; }       
    
    public string? Provider { get; set; } // Stripe / Paymob / Paypal
    public string? ProviderTransactionId { get; set; }
    public string? ProviderPaymentIntentId { get; set; }
    public string? IdempotencyKey { get; set; }

    public string? FailureReason { get; set; }
    public string? RawResponseJson { get; set; }

}
