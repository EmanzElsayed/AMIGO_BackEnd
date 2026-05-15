using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface ITranslationEntity
    {
        SupportedLanguage Language { get; set; }
    }
}
