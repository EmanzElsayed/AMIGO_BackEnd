using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Authentication
{
    public record ConfirmEmailRequestDTO
    (
        string Email ,
        string Token
    );
}
