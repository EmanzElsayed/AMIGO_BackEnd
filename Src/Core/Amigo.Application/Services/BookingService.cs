using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.OrderSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Booking;
using Amigo.Domain.DTO.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class BookingService(IUnitOfWork _unitOfWork, EncryptionService _encryptionService
                    ) :IBookingService
    {
        

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


        public async Task FinalizeBooking(Guid paymentId)
        {
            var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();
            var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();
            var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();



            var payment = await paymentRepo.GetByIdAsync(paymentId);


            // 2. Order 
            var order = await orderRepo.GetByIdAsync(new GetOrderByIdSpecification(payment.OrderId));

            if (order is null)
                return;

            order.Status = OrderStatus.Confirmed;

            // 3. Reservations 
            var reservations = await reservationRepo
                .GetAllAsync(new GetAllSlotReservationWithOrderIdSpecification(payment.OrderId));

            if (reservations is null)
                return;

            foreach (var r in reservations)
                r.Status = ReservationStatus.Confirmed;

            // 5. Batch check bookings
            var orderItemIds = order.OrderItems.Select(x => x.Id).ToList();

            var existingBookings = await bookingRepo.GetAllAsync(
                new GetBookingsByOrderItemIdsSpecification(orderItemIds));

            var existingSet = existingBookings.Select(x => x.OrderItemId).ToHashSet();

            List<Booking> bookingList = new List<Booking>(order.OrderItems.Count);
            foreach (var item in order.OrderItems)
            {
                if (existingSet.Contains(item.Id))
                    continue;

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    OrderItemId = item.Id,
                    OrderItem = item,
                    UserId = order.UserId,
                    CustomerName = order.User.FullName,
                    CustomerEmail = order.User.Email,
                    PaymentId = payment.Id,
                    BookingNumber = GenerateBookingNumber(),
                    Status = BookingStatus.Confirmed,
                    ConfirmedAt = DateTime.UtcNow,
                    Travelers = BuildTravelers(item)
                };
                bookingList.Add(booking);

            }
            await bookingRepo.AddRangeAsync(bookingList);

            var travelersDraftRepo = _unitOfWork.GetRepository<TravelerDraft, Guid>();

            var travelersDraft = order.OrderItems
                     .SelectMany(x => x.TravelersDraft)
                     .ToList();

            travelersDraftRepo.RemoveRange(travelersDraft);

            // Clear user's cart
            
            var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();

            var cart = await cartRepo.GetByIdAsync(new GetCartWithUserIdSpecification(order.UserId));
            if (cart != null && cart.Items != null)
            {
                await _unitOfWork.CartItemsRepo.BulkSoftDeleteByUserId(order.UserId);
                
            }


            await _unitOfWork.SaveChangesAsync();
        }

       
        private List<Traveler> BuildTravelers(
          OrderItem item)
        {
            return item.TravelersDraft.Select(t =>
            {
                var rawPassport = string.IsNullOrWhiteSpace(t.PassportNumber) ? null : t.PassportNumber.Trim();
                if (rawPassport != null && rawPassport.Length > 60)
                {
                    rawPassport = rawPassport.Substring(0, 60);
                }

                string finalPassport = null;
                if (rawPassport != null)
                {
                    try
                    {
                        _encryptionService.Decrypt(rawPassport);
                        finalPassport = rawPassport;
                    }
                    catch
                    {
                        finalPassport = _encryptionService.Encrypt(rawPassport);
                    }
                }

                return new Traveler
                {
                    Id = Guid.NewGuid(),
                    FullName = t.FullName,
                    Nationality = t.Nationality,
                    Type = t.Type,
                    BirthDate = t.BirthDate,
                    PassportNumber = finalPassport,
                };
            }).ToList();
        }

        private string GenerateBookingNumber()
        {
            return $"AMG-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }
    }
}
