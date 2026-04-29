using Amigo.Domain.DTO.CountryInfo;
using Amigo.SharedKernal.DTOs.Currency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICountryInfoService
    {
        Task<Result<List<GetCountryInfoResponseDTO>>> GetAllCountryInfoAsync(GetAllCountryInfoQuery requestQuery);

        Task<Result<GetCountryInfoResponseDTO>> GetCountryInfoByIdAsync(string countryInfoId, GetLanuageQuery query);
    }
}
