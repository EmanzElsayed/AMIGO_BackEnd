namespace Amigo.SharedKernal.DTOs.Tour;

public record CheckoutQuoteTierRequestDto(Guid PriceId, int Quantity);

public record CheckoutQuoteRequestDto(
    Guid TourId,
    Guid SlotId,
    string DateIso,
    IReadOnlyList<CheckoutQuoteTierRequestDto> Tiers,
    string? Language,
    string? Currency = null,
    string? EffectiveUserType = null);

public record CheckoutQuoteTierLineDto(
    Guid PriceId,
    string Label,
    decimal UnitAmount,
    int Quantity,
    decimal LineTotal,
    bool IsFree);

public record CheckoutQuoteResponseDto(
    decimal TotalAmount,
    string CurrencyCode,
    IReadOnlyList<CheckoutQuoteTierLineDto> Lines);
