namespace Amigo.Domain.Enum;
public enum OrderStatus
{
    PendingPayment = 1,
    Paid = 2,
    Confirmed = 3,
    Cancelled = 4,
    Refunded = 5,
    Failed = 6,
    Expired = 7
}
