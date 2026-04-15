using Amigo.Domain.DTO.Currency;
using Amigo.SharedKernal.DTOs.Currency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICurrencyService
    {
        Task<Result<List<GetCurrencyResponseDTO>>> GetAllCurrencyAsync(GetAllCurrencyQuery requestQuery);
        
        Task<Result<GetCurrencyResponseDTO>> GetCurrencyByIdAsync(string currencyId, GetAllCurrencyQuery requestQuery);
        Task<Result> CreateCurrencyAsync(CreateCurrencyRequestDTO requestDTO);
        Task<Result> UpdateCurrency(UpdateCurrencyRequestDTO requestDTO, string Id);

    }
}
