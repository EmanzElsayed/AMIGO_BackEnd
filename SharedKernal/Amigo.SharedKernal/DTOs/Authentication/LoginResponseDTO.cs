using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Authentication
{
    public record LoginResponseDTO
    (
        string Email,
        string Token,
        string Role
    );
}
