using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface ITranslatable<TTranslation>
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}
