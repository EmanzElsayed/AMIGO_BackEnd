using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ILocalizationService
    {
        string Get(string key);

        string Get(string key, params object[] arguments);
    }
}
