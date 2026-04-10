using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface INotIncludedMapping

    {
        List<TourNotIncluded>? TourNotIncludesToEntity(List<string> Includes, Tour tour, string language);

    }
}
