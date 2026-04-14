using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IInclusionMapping
    {
        //List<TourIncluded>? TourInclusionToEntity(List<string> Includes, Tour tour ,string language);
        List<TourInclusion>? TourInclusionToEntity(
                    List<string>? includedList,
                    List<string>? excludedList,
                    Tour tour,
                    string language
                    );
    }
}
