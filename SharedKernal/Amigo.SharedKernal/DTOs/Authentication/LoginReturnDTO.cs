using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Authentication
{
    public record LoginReturnDTO
    (
        string FullName,
        string Email ,
        string Token,
        string Role

    );
}
