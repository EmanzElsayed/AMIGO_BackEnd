using Amigo.Application.Abstraction.Services;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizationService(
            IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string Get(string key)
        {
            return _localizer[key];
        }

        public string Get(string key, params object[] arguments)
        {
            return _localizer[key, arguments];
        }
    }
}
