using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class BookingService(IUnitOfWork _unitOfWork):IBookingService
    {
        public async Task<Result<TravelersSavedResponseDTO>> AddTravelersAsync(
            Guid bookingId,
            string userId,
            AddTravelersRequestDTO dto)
        {
            var repo = _unitOfWork.GetRepository<Booking, Guid>();

            var booking = await repo.GetByIdAsync(
                 (new GetBookingByIdSpecification(bookingId)));

            if (booking is null)
                return Result.Fail("Booking not found");

            if (booking.UserId != userId)
                return Result.Fail(new UnauthorizedError("Unauthorized"));

            if (booking.Status != BookingStatus.Confirmed)
                return Result.Fail("Booking not confirmed");

            booking.Travelers.Clear();

            foreach (var item in dto.Travelers)
            {
                booking.Travelers.Add(new Traveler
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    FullName = item.FullName,
                    Nationality = item.Nationality,
                    BirthDate = item.BirthDate,
                    PassportNumber = item.PassportNumber,
                    PhoneNumber = item.PhoneNumber
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new TravelersSavedResponseDTO(
                booking.Id,
                booking.Travelers.Count,
                true
            ));
        }
    }
}
