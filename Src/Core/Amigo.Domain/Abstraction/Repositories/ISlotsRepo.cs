using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ISlotsRepo
    {
        Task<HashSet<Guid>> ReserveBulkAsync(
         IReadOnlyCollection<SlotReservationRequest> requests,
         CancellationToken ct = default);
      

        Task BulkDecreaseReservedCountAsync(
            IReadOnlyCollection<(Guid SlotId, int Quantity)> updates,
            CancellationToken ct = default);
}   }
