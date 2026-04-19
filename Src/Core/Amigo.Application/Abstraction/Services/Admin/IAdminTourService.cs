using Amigo.Domain.DTO.Tour;
using Amigo.SharedKernal.DTOs.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminTourService
    {
        Task<Result<CreateTourResponseDTO>> CreateTourAsync(CreateTourRequestDTO requestDTO);
        Task<Result> UpdateTourAsync(UpdateTourRequestDTO requestDTO, string tourId);
        Task<Result<PaginatedResponse<AdminTourListItemResponseDTO>>> GetAllToursAsync(GetAllAdminTourQuery requestQuery);
        Task<Result<GetTourResponseDTO>> GetTourById(string id, GetTourByIdRequestDTO requestDTO);
    }
}
