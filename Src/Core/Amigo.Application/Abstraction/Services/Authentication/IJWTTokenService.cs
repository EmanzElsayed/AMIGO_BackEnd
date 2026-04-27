using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Authentication
{
    public interface IJWTTokenService
    {
        Task<string> GenerateToken(ApplicationUser User);
        string GenerateRefreshToken();

    }
}
