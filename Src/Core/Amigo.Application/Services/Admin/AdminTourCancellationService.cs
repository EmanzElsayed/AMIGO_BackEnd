using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourCancellationService(IUnitOfWork _unitOfWork) : IAdminTourCancellationService
    {
        public async Task UpdateCancellationAsync(
              Tour tour,
             List<UpdateCancellationRequestDTO>? dto,
              SupportedLanguage? language)
        {
            if (dto is null)
                return ;


            var cancellationRepo = _unitOfWork.GetRepository<Cancellation, Guid>();
            //  ضمان وجود object
            var existingCancellations = await cancellationRepo.GetAllAsync(new GetCancellationWithTourIdSpecification(tour.Id));
            if (existingCancellations is not null && existingCancellations.Any())
            {
                cancellationRepo.RemoveRange(existingCancellations);
            }

            var cancellations = dto.Select(c => new Cancellation()
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                TourId = tour.Id,
                RefundPercentage = c.RefundPercentage ,
                CancelationPolicyType = Enum.Parse<CancelationPolicyType> (c.CancelationPolicyType),
                CancellationBefore = c.CancellationBefore,



            });



            await cancellationRepo.AddRangeAsync(cancellations);
            

        }
        
    }
}
