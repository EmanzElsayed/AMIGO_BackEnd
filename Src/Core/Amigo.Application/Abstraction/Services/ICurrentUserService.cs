using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        SupportedLanguage Language { get; }
        CurrencyCode Currency {  get; }
    }
}
