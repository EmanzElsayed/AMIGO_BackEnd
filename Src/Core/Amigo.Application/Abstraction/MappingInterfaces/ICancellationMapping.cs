using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface ICancellationMapping
    {
        Cancellation CancellationToEntity(CreateCancellationRequestDTO requestDTO, Tour tour,string language);
        //CancellationTranslation CancellationTranslationToEntity(CreateCancellationRequestDTO requestDTO, Cancellation cancellation, string language);

    }
}
