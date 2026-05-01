using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.AvailableSlots
{
    public sealed record SlotReservationRequest(
    Guid SlotId,
    int Quantity);
}
