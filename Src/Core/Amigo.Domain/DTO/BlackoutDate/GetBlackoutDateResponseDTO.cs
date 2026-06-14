using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.BlackoutDate
{
    public record GetBlackoutDateResponseDTO
    (
       Guid Id,
      DateOnly Date
    );
}
