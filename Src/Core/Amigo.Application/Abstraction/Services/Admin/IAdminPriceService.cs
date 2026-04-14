using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Amigo.SharedKernal.DTOs.Price;
using Amigo.SharedKernal.DTOs.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminPriceService
    {
        //Task<Result<CreatePriceResponseDTO>> CreatePriceAsync(CreatePriceRequestDTO requestDTO);
        Task UpdatePricesAsync(
                  Tour tour,
                  List<UpdatePriceRequestDTO> prices,
                  Language? language);
    }
}
