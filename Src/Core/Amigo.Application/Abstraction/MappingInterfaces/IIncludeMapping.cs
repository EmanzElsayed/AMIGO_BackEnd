using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IIncludeMapping
    {
        List<TourIncluded>? TourIncludesToEntity(List<string> Includes, Tour tour ,string language);
    }
}
