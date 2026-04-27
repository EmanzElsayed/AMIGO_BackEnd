using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Authentication
{
    public record AuthResponseDTO(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpires
    );
}
