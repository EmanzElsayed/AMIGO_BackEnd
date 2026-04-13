using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminTourCancellationService
    {
        Task UpdateCancellationAsync(
               Tour tour,
               UpdateCancellationRequestDTO? dto,
               Language? language);
    }
}
