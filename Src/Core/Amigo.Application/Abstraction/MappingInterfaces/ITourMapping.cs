using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface ITourMapping
    {
        Tour TourToEntity(CreateTourRequestDTO requestDTO , Destination destination);
        TourTranslation TourTranslationToEntity(CreateTourRequestDTO requestDTO, Tour tour);

        void UpdateTour(
                        UpdateTourRequestDTO requestDTO,
                        Tour tour,
                        TourTranslation? translation,
                        Language? language);
    }
}
