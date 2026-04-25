using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.DTO.Cart;
using Amigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Services
{
    public class CartService(IUnitOfWork _unitOfWork , IPaymentProviderResolver _paymentProviderResolver,ISlotsRepo _slotsRepo) 
        : ICartService
    {
        public async Task<Result<CartDTO>> GetCurrentCartAsync(string? userId, string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken);

            var MappedCart = cart.ToDto();

            return Result.Ok(MappedCart);
        }

        public async Task<Result<CartDTO>> AddItemAsync(
                string? userId,
                string? cartToken,
                AddCartItemRequestDTO requestDTO)
        {
            var cart = await GetOrCreateCart(userId, cartToken);


            var tour = await _unitOfWork.GetRepository<Tour,Guid>().GetByIdAsync(new GetTourByIdSpecification(requestDTO.TourId));
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            }

            var slot = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetByIdAsync(new GetAvaialableSlotsByIdSpecification(requestDTO.SlotId));
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Slot Not Found"));

            }
               var translatedTitle = tour.Translations
                   .FirstOrDefault(x => x.Language == requestDTO.Language)?.Title
                   ?? tour.Translations.FirstOrDefault()?.Title
                   ?? "Tour";
                var destinationName = tour.Destination?.Translations?
                .FirstOrDefault(t => t.Language == requestDTO.Language)?.Name
                ?? tour.Destination?.Translations?.FirstOrDefault()?.Name
                ?? string.Empty;
            var item = new CartItem
            {
                //Id = Guid.NewGuid(),
                Cart = cart,
                CartId = cart.Id,
                TourId = requestDTO.TourId,
                Tour = tour,
                SlotId = requestDTO.SlotId,
                Slot = slot,
                Language = requestDTO.Language,
                TourDate = requestDTO.TourDate,
                StartTime = requestDTO.StartTime,
                TourTitle = translatedTitle,
    
                DestinationName = destinationName
            };

            foreach (var p in requestDTO.Prices)
            {
                var retailPrice = GetPriceFromTour(tour, p.Type,requestDTO.Language);

                item.Prices.Add(new CartPrice
                {
                    //Id = Guid.NewGuid(),
                    Type = p.Type,
                    Quantity = p.Quantity,
                    RetailPrice = retailPrice
                });
            }

            item.TotalAmount = item.Prices.Sum(x => x.FinalPrice);

            cart.Items.Add(item);

            RecalculateCart(cart);

            await _unitOfWork.SaveChangesAsync();


            return cart.ToDto();
        }

        public async Task<Result<CartDTO>> UpdateItemAsync(
               Guid itemId,
               string? userId,
               string? cartToken,
               UpdateCartItemRequestDTO dto)
        {
            var cart = await GetOrCreateCart(userId, cartToken);

            if(!cart.Items.Any())
                return Result.Fail(new NotFoundError("Cart don't have items"));

            var item = cart.Items.FirstOrDefault(x => x.Id == itemId);

                if(item is null)
                return Result.Fail(new NotFoundError("This Item Not Found"));

            item.Prices.Clear();

            foreach (var p in dto.Prices)
            {
                var retailPrice = GetPriceFromTour(item.Tour, p.Type, item.Language);

                item.Prices.Add(new CartPrice
                {
                    Id = Guid.NewGuid(),
                    Type = p.Type,
                    Quantity = p.Quantity,
                    RetailPrice = retailPrice
                });
            }

            item.TotalAmount = item.Prices.Sum(x => x.FinalPrice);

            RecalculateCart(cart);

            await _unitOfWork.SaveChangesAsync();

            return cart.ToDto();
        }


        public async Task<Result<CheckoutResponseDTO>> CheckoutAsync(string userId, string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken);

            if (!cart.Items.Any())
                return Result.Fail("Cart don't have Items");

            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Currency = cart.CurrencyCode,
                        Status = OrderStatus.PendingPayment,
                        OrderDate = DateTime.UtcNow,
                        TotalAmount = 0,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(20),
                        OrderItems = new List<OrderItem>(cart.Items.Count)
                    };

                    var response = new CheckoutResponseDTO(
                        OrderId: order.Id,
                        ChangedPrices: new List<CheckoutPriceResponseDTO>(),
                         PaymentId: Guid.Empty
                    );

                    decimal total = 0;

                    var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();

                    var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

                    var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();


                    // GEt Ids

                    var tourIds = cart.Items.Select(x => x.TourId).Distinct().ToList();
                    var slotIds = cart.Items.Select(x => x.SlotId).Distinct().OrderBy(x => x).ToList();
                    var orderId = order.Id;

                    //create tour dictionary

                    var tours =  await tourRepo.GetAllAsync(
                                     new GetToursByIdsSpecification(tourIds));



                    var slots = await slotRepo.GetAllAsync(
                             new GetSlotsByIdsSpecification(slotIds));


                    //var slots = await _slotsRepo.GetLockedSlotsAsync(slotIds);

                    if (slots is null || !slots.Any() ) return Result.Fail("Slots Not Avaialble");

                    var reservations = await reservationRepo.GetAllAsync(
                        new GetReservationsBySlotIdsSpecification(slotIds));


                    var tourDict = tours.ToDictionary(x => x.Id);

                    var priceDict = tours
                            .SelectMany(t => t.Prices.SelectMany(p =>
                                p.Translations.Select(tr => new
                                {
                                    TourId = t.Id,
                                    Price = p,
                                    Type = tr.Type,
                                    Language = tr.Language
                                })))
                            .ToLookup(x => (x.TourId, x.Type, x.Language), x => x.Price);


                    //create slots dictionary



                    var slotDict = slots.ToDictionary(x => x.Id);

                    //create reservations dictionary



                    var reservationLookup = reservations
                        .GroupBy(x => x.SlotId)
                        .ToDictionary(x => x.Key, x => x.Sum(r => r.Quantity));

                    var newReservations = new List<SlotReservation>(cart.Items.Count);

                    foreach (var item in cart.Items)
                    {
                        if (!tourDict.TryGetValue(item.TourId, out var tour))
                            return Result.Fail($"Tour {item.TourId} not found");

                        if (!slotDict.TryGetValue(item.SlotId, out var orderedSlot))
                            return Result.Fail("Slot not found");

                        var reserved = reservationLookup.GetValueOrDefault(item.SlotId, 0);
                        
                        var available =
                            orderedSlot.MaxCapacity -
                            reserved;


                        var prices = item.Prices;
                        var totalPeople = prices.Sum(x => x.Quantity);

                        if (totalPeople > available)
                            return Result.Fail($"Slot full, available {available}");

                        reservationLookup[item.SlotId] =
                                               reservationLookup.GetValueOrDefault(item.SlotId, 0) + totalPeople;


                        // reserved  slot 
                        var reservation = new SlotReservation
                        {
                            Id = Guid.NewGuid(),
                            SlotId = orderedSlot.Id,
                            OrderId = order.Id,
                            Quantity = totalPeople,
                            CreatedAt = DateTime.UtcNow,
                            ExpiresAt = DateTime.UtcNow.AddMinutes(20),
                            Status = ReservationStatus.Pending
                        };
                        newReservations.Add(reservation);


                        string tourTitle =
                            tour.Translations
                                .FirstOrDefault(t => t.Language == item.Language)
                                ?.Title ?? "Tour";

                        string destinationName =
                            tour.Destination.Translations
                                .FirstOrDefault(t => t.Language == item.Language)
                                ?.Name ?? "";

                        if (!string.IsNullOrWhiteSpace(tourTitle) && SlugHelper.MatchesName(tourTitle, item.TourTitle) == false)
                                        
                        {
                            response = response with
                            {
                                IsTourTitleChanged = true
                            };
                        }

                        if (!string.IsNullOrWhiteSpace(destinationName) &&
                            !destinationName.Trim().ToLower()
                                .Contains(item.DestinationName.ToLower()))
                        {
                            response = response with
                            {
                                IsDestinationNameChanged = true
                            };
                        }

                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            TourId = tour.Id,
                            SlotId = item.SlotId,
                            TourDate = item.TourDate,
                            StartTime = item.StartTime,
                            CurrencyCode = cart.CurrencyCode,
                            Language = item.Language,
                            MeetingPoint = tour.MeetingPoint,
                            Duration = tour.Duration,
                            TourTitle = tourTitle,
                            DestinationName = destinationName,
                            CancelationPolicyType =
                                tour.Cancellation.CancelationPolicyType,
                            CancellationBefore =
                                tour.Cancellation.CancellationBefore,
                            RefundPercentage =
                                tour.Cancellation.RefundPercentage
                        };

                        decimal itemTotal = 0;

                        foreach (var cartPrice in prices)
                        {
                            var priceEntity =  priceDict[(tour.Id, cartPrice.Type, item.Language)]
                                                                    .SingleOrDefault(); 

                            if (priceEntity is null)
                                return Result.Fail(
                                    $"Price type {cartPrice.Type} not found for tour {tour.Id}");

                            var currentRetailPrice = priceEntity.RetailPrice;

                            if (cartPrice.RetailPrice != currentRetailPrice)
                            {
                                response.ChangedPrices.Add(
                                    new CheckoutPriceResponseDTO(
                                        cartPrice.Type,
                                        currentRetailPrice));
                            }

                            orderItem.OrderedPrice.Add(new OrderedPrice
                            {
                                Id = Guid.NewGuid(),
                                Type = cartPrice.Type,
                                Quantity = cartPrice.Quantity,
                                RetailPrice = currentRetailPrice
                            });

                            itemTotal += currentRetailPrice * cartPrice.Quantity;
                        }

                        total += itemTotal;

                        order.OrderItems.Add(orderItem);
                    }

                    order.TotalAmount = total;

                    // Save Order
                    await _unitOfWork
                        .GetRepository<Order, Guid>()
                        .AddAsync(order);

                    // Save new Reservation 
                    await reservationRepo.AddRangeAsync(newReservations);

                    // -------------------------------
                    // Create Payment Record
                    // -------------------------------
                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        TotalAmount = total,
                        Status = PaymentStatus.Pending,
                        Currency = cart.CurrencyCode
                    };

                    await _unitOfWork
                        .GetRepository<Payment, Guid>()
                        .AddAsync(payment);


                   

                    // -------------------------------
                    // Clear Cart
                    // -------------------------------
                    _unitOfWork
                        .GetRepository<CartItem, Guid>()
                        .RemoveRange(cart.Items);

                    cart.TotalAmount = 0;

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result.Ok(
                        response with
                        {
                            PaymentId = payment.Id
                           
                        });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return FluentValidationExtension
                        .FromException(details: ex.Message);
                }
            });
        }
        private void RecalculateCart(Cart cart)
        {
            cart.TotalAmount = cart.Items.Sum(x => x.TotalAmount);
            cart.LastUpdatedAt = DateTime.UtcNow;
        }
        private decimal GetPriceFromTour(Tour tour, string type, Language lang)
        {
            var price = tour.Prices
                .FirstOrDefault(p =>
                    p.Translations.Any(t =>
                        t.Type == type &&
                        t.Language == lang));

            if (price == null)
                return 0;

            return price.RetailPrice;
        }
        private async Task<Cart> GetOrCreateCart(string? userId, string? cartToken)
        {

            Cart? cart = null;
            var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
            if (!string.IsNullOrWhiteSpace(userId))
                cart = await cartRepo.GetByIdAsync(new GetCartWithUserIdSpecification(userId));

            if (cart is null && !string.IsNullOrWhiteSpace(cartToken))
                cart = await cartRepo.GetByIdAsync(new GetCartWithCartTokenSpecification(cartToken));

             if (cart is not  null)
                        return cart;
           
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CartToken = cartToken ?? Guid.NewGuid().ToString(),
                CurrencyCode = CurrencyCode.USD,
                LastUpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(15)
            };
            await cartRepo.AddAsync(cart);

            await _unitOfWork.SaveChangesAsync();
            
            return cart;

        }

    }
}
