using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record PiceWithActivityTypeRequestDTO
    (
       Guid TourId,
       string ActivityType
     );
}
