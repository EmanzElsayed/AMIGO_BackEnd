using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Authentication
{
    public record ResetPasswordRequestDTO
    (
        string Email,
         string Token,
         string NewPassword,
         string ConfirmPassword
    );
}
