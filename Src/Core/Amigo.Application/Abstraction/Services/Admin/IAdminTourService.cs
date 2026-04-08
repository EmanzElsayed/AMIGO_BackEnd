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

    }
}
