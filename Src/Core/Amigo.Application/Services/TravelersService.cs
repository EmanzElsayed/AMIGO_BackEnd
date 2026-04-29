using Amigo.Application.Specifications.TravelerSpecification;
using Amigo.Domain.DTO.Travelers;
using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class TravelersService(IUnitOfWork _unitOfWork,EncryptionService _encryptionService) : ITravelersService
    {
        public async Task<Result<List<GetTravelerResponsDTO>>> GetAllTravelers(string UserId, GetAllTravelersQuery query)
        {


            var travelerDraftSpec = new GetAllDraftTravelersSpecification(query, UserId);
            var travelersDraft =  await _unitOfWork.GetRepository<TravelerDraft, Guid>().GetAllAsync(travelerDraftSpec);


            var travelerSpec = new GetAllTravelersSpecification(query, UserId);

            var travelers = await _unitOfWork.GetRepository<Traveler, Guid>().GetAllAsync(travelerSpec);



            var response = (travelersDraft is null || !travelersDraft.Any()) ? new List<GetTravelerResponsDTO>(): 
                travelersDraft.Select(td => new GetTravelerResponsDTO(
                    TravelerId: td.Id,
                    FirstName: FirstName(td.FullName),
                    LastName: LastName(td.FullName),
                    Nationality: td.Nationality,
                    PassportNumber: string.IsNullOrWhiteSpace(td.PassportNumber)
                        ? null
                        : _encryptionService.Decrypt(td.PassportNumber) ,
                    BirthDate: td.BirthDate ?? null
                )).ToList();
            if (travelers.Any())
            { 
            
                response.AddRange(
                        travelers.Select(td => new GetTravelerResponsDTO(
                        TravelerId: td.Id,
                        FirstName: FirstName(td.FullName),
                        LastName: LastName(td.FullName),
                        Nationality: td.Nationality,
                        PassportNumber: string.IsNullOrWhiteSpace(td.PassportNumber)
                        ? null
                        : _encryptionService.Decrypt(td.PassportNumber)     ,
                        BirthDate: td.BirthDate ?? null

                            ))
                    );
            }
            
            return response;
        }

        private string FirstName(string fullName)
        {
            var parts = fullName
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

              return parts.First();

        }
        private string LastName(string fullName)
        {
            var parts = fullName
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return parts.Length == 1
                     ? ""
                        : string.Join(" ", parts.Skip(1)); 

        }
        public async Task<Result<GetTravelerResponsDTO>> GetTravelerById(string UserId, string Id)
        {

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid travelerId = guid;

            var travelerDraftSpec = new GetTravelerDraftById(UserId,travelerId);
            var travelerDraft = await _unitOfWork.GetRepository<TravelerDraft, Guid>().GetByIdAsync(travelerDraftSpec);
            if (travelerDraft is null)
            {
                var traveler = await _unitOfWork.GetRepository<Traveler, Guid>().GetByIdAsync(new GetTravelerByIdSpecification(UserId, travelerId));
                if (traveler is null)
                {
                    return Result.Fail(new NotFoundError("This Traveler  Not Found"));
                }
                return Result.Ok(new GetTravelerResponsDTO(
                    TravelerId: traveler.Id,
                        FirstName: FirstName(traveler.FullName),
                        LastName: LastName(traveler.FullName),
                        Nationality: traveler.Nationality,
                        PassportNumber: string.IsNullOrWhiteSpace(traveler.PassportNumber)
                        ? null
                        : _encryptionService.Decrypt(traveler.PassportNumber),
                        BirthDate: traveler.BirthDate ?? null
                    ));
            }
            else
            {
                return Result.Ok(new GetTravelerResponsDTO(
                    TravelerId: travelerDraft.Id,
                        FirstName: FirstName(travelerDraft.FullName),
                        LastName: LastName(travelerDraft.FullName),
                        Nationality: travelerDraft.Nationality,
                        PassportNumber: string.IsNullOrWhiteSpace(travelerDraft.PassportNumber)
                        ? null
                        : _encryptionService.Decrypt(travelerDraft.PassportNumber)     ,              
                        BirthDate: travelerDraft.BirthDate ?? null
                    ));
            }

        }
    }
}
