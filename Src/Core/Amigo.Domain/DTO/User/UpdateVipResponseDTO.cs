using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.User
{
    public record UpdateVipResponseDTO
    (
        string Id,
        bool IsVIP
    );
}
