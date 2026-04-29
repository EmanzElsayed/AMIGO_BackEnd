using Amigo.Domain.DTO.Travelers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ITravelersService
    {
        Task<Result<List<GetTravelerResponsDTO>>> GetAllTravelers(string UserId,GetAllTravelersQuery query);
        Task<Result<GetTravelerResponsDTO>> GetTravelerById(string UserId,string travelerId);

    }
}
