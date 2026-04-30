using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.TourSpecification;
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

        public async Task<Result<IEnumerable<UserBookingDTO>>> GetUserBookingsAsync(string userId, string? paymentStatus = null)
        {
            var itemRepo = _unitOfWork.GetRepository<OrderItem, Guid>();
            var spec = new GetUserBookingHistorySpecification(userId, paymentStatus);
            var items = await itemRepo.GetAllAsync(spec);

            if (items == null || !items.Any())
                return Result.Ok(Enumerable.Empty<UserBookingDTO>());

            if (!string.IsNullOrWhiteSpace(paymentStatus) && paymentStatus.Equals("Succeeded", StringComparison.OrdinalIgnoreCase))
            {
                items = items.Where(i => i.Order.Status == OrderStatus.Paid && 
                                        i.Order.Payments.Any(p => p.Status == PaymentStatus.Succeeded && !p.IsDeleted));
            }

            var tourIds = items.Where(i => i.TourId.HasValue).Select(i => i.TourId!.Value).Distinct().ToList();
            
            var imageRepo = _unitOfWork.GetRepository<TourImage, Guid>();
            var images = await imageRepo.GetAllAsync(); 
            
            var imageMap = images
                .Where(img => tourIds.Contains(img.TourId) && !img.IsDeleted)
                .GroupBy(img => img.TourId)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.CreatedDate).Select(x => x.ImageUrl).FirstOrDefault());

            var dtos = items.Select(item => {
                var payment = item.Order.Payments.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
                return new UserBookingDTO
                {
                    OrderId = item.OrderId,
                    ItemId = item.Id,
                    TourId = item.TourId ?? Guid.Empty,
                    TourTitle = item.TourTitle,
                    DestinationName = item.DestinationName,
                    DateIso = item.TourDate,
                    StartTime = item.StartTime.ToString("HH:mm"),
                    PaymentStatus = payment?.Status ?? PaymentStatus.Pending,
                    OrderStatus = item.Order.Status,
                    PaidAmount = payment?.TotalAmount ?? 0,
                    Currency = payment?.Currency.ToString() ?? item.Order.Currency.ToString(),
                    ImageUrl = item.TourId.HasValue ? imageMap.GetValueOrDefault(item.TourId.Value) : null
                };
            });

            return Result.Ok(dtos);
        }
    }
}
