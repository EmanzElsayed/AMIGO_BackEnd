using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Abstraction.Services;

public interface ICheckoutQuoteService
{
    Task<Result<CheckoutQuoteResponseDto>> QuoteAsync(CheckoutQuoteRequestDto request);
}
