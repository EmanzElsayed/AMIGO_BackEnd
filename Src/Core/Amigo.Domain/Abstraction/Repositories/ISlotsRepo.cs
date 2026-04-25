using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ISlotsRepo
    {
        Task<List<AvailableSlots>?> GetLockedSlotsAsync(List<Guid> slotsIds);
        Task<bool> TryReserveSlotAsync(Guid slotId, int qty);

    }
}
