using Amigo.Application.Abstraction.Services;
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
    public class CartService(IUnitOfWork _unitOfWork , IPaymentService _paymentService) : ICartService
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
                        TotalAmount = 0
                    };

                    var response = new CheckoutResponseDTO(
                        OrderId: order.Id,
                        ChangedPrices: new List<CheckoutPriceResponseDTO>(),
                         ClientSecret: string.Empty,
                         PaymentId: Guid.Empty
                    );

                    decimal total = 0;

                    var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();

                    foreach (var item in cart.Items)
                    {
                        var tour = await tourRepo.GetByIdAsync(
                            new GetTourByIdSpecification(item.TourId));


                        if (tour is null)
                            return Result.Fail(
                                new NotFoundError($"Tour {item.TourId} not found"));


                        var orderedSlot = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetByIdAsync(new GetAvaialableSlotsByIdSpecification(item.SlotId));
                                

                        if (orderedSlot is null)
                                   return Result.Fail("This time is no longer available");


                        // check if available place in time  
                        var reservedSlots = await _unitOfWork.GetRepository<SlotReservation, Guid>()
                        .GetAllAsync(new GetAllSlotReservationSpecification(orderedSlot.Id));
                        var reserved =  reservedSlots.Sum(s => s.Quantity);               
                            

                        var available = orderedSlot.MaxCapacity - orderedSlot.BookedCount - reserved;

                        int totalPeople = item.Prices.Sum(x => x.Quantity);

                        if ( totalPeople > available)
                            return Result.Fail($"Slot is full avialable {available} seats at this time");

                        // reserved  slot 
                        var reservation = new SlotReservation
                        {
                            Id = Guid.NewGuid(),
                            SlotId = orderedSlot.Id,
                            OrderId = order.Id,
                            Quantity = totalPeople,
                            CreatedAt = DateTime.UtcNow,
                            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                            Status = ReservationStatus.Pending
                        };

                        await _unitOfWork.GetRepository<SlotReservation,Guid>().AddAsync(reservation);


                        string tourTitle =
                            tour.Translations
                                .FirstOrDefault(t => t.Language == item.Language)
                                ?.Title ?? "Tour";

                        string destinationName =
                            tour.Destination.Translations
                                .FirstOrDefault(t => t.Language == item.Language)
                                ?.Name ?? "";

                        if (!string.IsNullOrWhiteSpace(tourTitle) &&
                            !tourTitle.Trim().ToLower()
                                .Contains(item.TourTitle.ToLower()))
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

                        foreach (var cartPrice in item.Prices)
                        {
                            var priceEntity = tour.Prices.FirstOrDefault(p =>
                                p.Translations.Any(t =>
                                    t.Type == cartPrice.Type &&
                                    t.Language == item.Language));

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

                    // -------------------------------
                    // Create Payment Record
                    // -------------------------------
                    var payment = new Payment
                    {
                        //Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        TotalAmount = total,
                        Status = PaymentStatus.Pending,
                        Provider = "Stripe",
                        Currency = cart.CurrencyCode
                    };

                    await _unitOfWork
                        .GetRepository<Payment, Guid>()
                        .AddAsync(payment);

                    await _unitOfWork.SaveChangesAsync();

                    // -------------------------------
                    // Create Stripe PaymentIntent
                    // -------------------------------
                    var stripe =
                        await _paymentService.CreateStripePaymentAsync(order);

                    payment.ProviderPaymentIntentId =
                        stripe.PaymentIntentId;

                    await _unitOfWork.SaveChangesAsync();

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
                            PaymentId = payment.Id,
                            ClientSecret = stripe.ClientSecret
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
