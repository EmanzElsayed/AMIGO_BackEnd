using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface ITranslationEntity
    {
        Language Language { get; set; }
    }
}
