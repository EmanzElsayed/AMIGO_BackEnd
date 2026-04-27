using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Authentication
{
    public record LoginResponseDTO
    (
        string FullName,
        string Email,
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresIn,
        bool EmailConfirmed,
        string? Role 

    );
}
