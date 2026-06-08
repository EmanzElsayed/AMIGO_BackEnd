using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.Cart;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;


namespace Amigo.Application.Services
{
    public class CartService(IUnitOfWork _unitOfWork ,
        EncryptionService _encryptionService,
        ICurrencyRateService _currencyRateService,
        ISlotsRepo _slotsRepo,
        ICacheService _cacheService,
        Microsoft.AspNetCore.Identity.UserManager<Amigo.Domain.Entities.Identity.ApplicationUser> _userManager
        ) 
        : ICartService
    {
        public async Task<Result<CartDTO>> GetCurrentCartAsync(string? userId, string? cartToken)
        {
            var cacheKey =
            BuildCartCacheKey(userId, cartToken);

            // 1. Try Cache
            var cached =
                await _cacheService
                    .GetAsync<CartDTO>(cacheKey);

            if (cached != null)
                return Result.Ok(cached);

            // 2. DB
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);

            if (cart == null || !cart.Items.Any())
            {
                return Result.Ok(new CartDTO(
                    Guid.Empty,
                    userId,
                    cartToken,
                    null,
                    0,
                    0,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(15),
                    new List<CartItemDTO>()
                ));
            }

            var MappedCart = cart.ToDto();
            // 3. Save Cache
            await _cacheService.SetAsync(
                cacheKey,
                MappedCart,
                TimeSpan.FromMinutes(25));

            return Result.Ok(MappedCart);
        }

        public async Task<Result<CartDTO>> AddItemAsync(
                string? userId,
                string? cartToken,
                AddCartItemRequestDTO requestDTO)
        {
            var cart = await GetOrCreateCart(userId, cartToken);

            CurrencyCode requestedCurrency = EnumsMapping.ToEnum<CurrencyCode>(requestDTO.RequestedCurrencyCode, false);

            if (!cart.Items.Any())
            {
                cart.CurrencyCode = requestedCurrency;


            }


            var tour = await _unitOfWork.GetRepository<Tour,Guid>().GetByIdAsync(new GetTourByIdSpecification(requestDTO.TourId));
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            }

            var slot = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetByIdAsync(new GetAvaialableSlotsByIdSpecification(requestDTO.SlotId));
            if (slot is null)
            {
                return Result.Fail(new NotFoundError("This Slot Not Found"));

            }
            if (slot.AvailableTimeStatus != AvailableDateTimeStatus.Available)
            {
                return Result.Fail("This Slot Not Available Now");

            }

            var requestedQty = requestDTO.Prices.Sum(x => x.Quantity);
            var remaining = slot.MaxCapacity - slot.ReservedCount;
            if (requestedQty > remaining)
            {
                return Result.Fail($"Sorry, only {remaining} spots are available for this time slot.");
            }
            var translatedTitle = tour.Translations
                   .FirstOrDefault(x => x.Language == requestDTO.Language)?.Title
                   ?? tour.Translations.FirstOrDefault()?.Title
                   ?? "Tour";
                var destinationName = tour.Destination?.Translations?
                .FirstOrDefault(t => t.Language == requestDTO.Language)?.Name
                ?? tour.Destination?.Translations?.FirstOrDefault()?.Name
                ?? string.Empty;
            string? activityType =  requestDTO.Prices
                                    .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ActivityType))?.ActivityType ?? null;

            var item = new CartItem
            {
                
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
    
                DestinationName = destinationName,
                ActivityType = activityType
            };

            var userType = await GetUserType(userId);

            var rate = await _currencyRateService.GetRateAsync(
                    Constants.BaseCurrency,
                    cart.CurrencyCode.Value,false);

            if (!rate.IsSuccess)
                return Result.Fail(rate.Errors);


            var prices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PricesForTourSpecification(tour.Id, userType));
            
            foreach (var p in requestDTO.Prices)
            {
                
                var retailPrice = GetPriceFromTour(prices, p.Type,requestDTO.Language, activityType);

                item.Prices.Add(new CartPrice
                {
                    
                    Type = p.Type,
                    Quantity = p.Quantity,
                    BaseRetailPrice = retailPrice,
                    ConvertedRetailPrice = retailPrice * rate.ValueOrDefault,
                    ExchangeRate = rate.ValueOrDefault

                });
            }
            item.ActivityType = activityType;
            item.TotalAmount = item.Prices.Sum(x => x.FinalPrice);

            cart.Items.Add(item);

            RecalculateCart(cart);

            await _unitOfWork.SaveChangesAsync();

            var dto = cart.ToDto();
            await _cacheService.SetAsync(
            BuildCartCacheKey(userId, cartToken),
            dto,
            TimeSpan.FromMinutes(25));

            return dto;
        }



        private async Task<UserType> GetUserType(string? userId)
        {


            if (userId is null) return UserType.Public;
            var user = await _userManager.FindByIdAsync(userId);

            string role = "User";

            if (user is not null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                role = roles.FirstOrDefault();
            }


            return role == "VIP" ? UserType.VIP : UserType.Public;
        }


        public async Task<Result<CartDTO>> UpdateItemAsync(
                Guid itemId,
                string? userId,
                string? cartToken,
                UpdateCartItemRequestDTO dto)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
            if (cart == null) return Result.Fail(new NotFoundError("Cart not found"));

            //var itemRepo = _unitOfWork.GetRepository<CartItem, Guid>();
            //var item = await itemRepo.GetByIdAsync(new GetCartItemWithIdSpecification(itemId));
            var item = cart.Items.FirstOrDefault(x => x.Id == itemId);

            if (item is null || item.CartId != cart.Id)
                return Result.Fail(new NotFoundError("This Item Not Found"));


            if (dto.Prices != null && dto.Prices.Any())
            {
                var requestedQty = dto.Prices.Sum(x => x.Quantity);
                var currentItemQty = item.Prices.Sum(x => x.Quantity);
                
                // We check if the INCREASE exceeds the remaining capacity
                // Slot is already attached to item
                var slot = await _unitOfWork.GetRepository<AvailableSlots,Guid>().GetByIdAsync(item.SlotId);
                if (slot is null )
                    return Result.Fail(new NotFoundError("This Slot Not Found"));

                var remaining = slot.MaxCapacity - slot.ReservedCount;
                
                if (requestedQty > currentItemQty && (requestedQty - currentItemQty) > remaining)
                {
                    return Result.Fail($"Sorry, only {remaining + currentItemQty} total spots are available.");
                }

                var priceRepo = _unitOfWork.GetRepository<CartPrice, Guid>();
                var cartPrices = await priceRepo.GetAllAsync(new GetCartPriceWithCartItemIdSpecification(itemId));
                priceRepo.RemoveRange(cartPrices);
                

                var userType = await GetUserType(userId);

                var rate = await _currencyRateService.GetRateAsync(
                Constants.BaseCurrency,
                cart.CurrencyCode.Value,false);

                if (!rate.IsSuccess)
                    return Result.Fail(rate.Errors);

                string? activityType = dto.Prices
                                                  .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ActivityType))?.ActivityType ?? null;

                var prices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PricesForTourSpecification(item.TourId, userType));

                List<CartPrice> newCartprices = new List<CartPrice>();

                foreach (var p in dto.Prices)
                {
                    var retailPrice = GetPriceFromTour(prices, p.Type, item.Language,activityType);

                    newCartprices.Add(new CartPrice
                    {
                        Id = Guid.NewGuid(),
                        CartItemId = item.Id,
                        CartItem = item,
                        Type = p.Type,
                        Quantity = p.Quantity,
                        BaseRetailPrice = retailPrice,
                        ConvertedRetailPrice = retailPrice * rate.ValueOrDefault,
                        ExchangeRate = rate.ValueOrDefault
                    });
                }
                await priceRepo.AddRangeAsync(newCartprices);
                item.ActivityType = activityType;
                item.TotalAmount = newCartprices.Sum(x => x.FinalPrice);

                RecalculateCart(cart);
            }

            List<TravelerDraft> travelers = new List<TravelerDraft>();
            if (dto.Travelers != null)
            {
                var travelerRepo = _unitOfWork.GetRepository<TravelerDraft, Guid>();
                item.Travelers.Clear();
                foreach (var t in dto.Travelers)
                {
                    var rawPassport = string.IsNullOrWhiteSpace(t.PassportNumber) ? null : t.PassportNumber.Trim();
                    if (rawPassport != null && rawPassport.Length > 60)
                    {
                        rawPassport = rawPassport.Substring(0, 60);
                    }

                    var newTraveler = new TravelerDraft
                    {
                        Id = Guid.NewGuid(),
                        CartItemId = item.Id,
                        FullName = $"{t.FirstName} {t.LastName}",
                        Nationality = t.Nationality,
                        Type = t.Type,
                        BirthDate = t.BirthDate,
                        PassportNumber = rawPassport == null ? null : _encryptionService.Encrypt(rawPassport)
                    };
                    travelers.Add(newTraveler);
                }
                    await travelerRepo.AddRangeAsync(travelers);
                    //item.Travelers.Add(newTraveler);
            }
           
         

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error($"DB Error: {ex.Message} Inner: {ex.InnerException?.Message}"));
            }


            var mappedCart = cart.ToDto();
            await _cacheService.SetAsync(
            BuildCartCacheKey(userId, cartToken),
            dto,
            TimeSpan.FromMinutes(25));

            return mappedCart;
        }




        public async Task<Result<CheckoutResponseDTO>> CheckoutAsync(CheckoutRequestDTO requestDTO,string userId, string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
            if (cart == null) return Result.Fail("Cart not found");

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
                        Currency = cart.CurrencyCode.GetValueOrDefault(),
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

                    var tourIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.TourId).ToHashSet();
                    var slotIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.SlotId).Distinct().OrderBy(x => x).ToList();
                    var orderId = order.Id;

                    //create tour dictionary

                    var tours =  await tourRepo.GetAllAsync(
                                     new GetToursByIdsSpecification(tourIds));



                    var slots = await slotRepo.GetAllAsync(
                             new GetSlotsByIdsSpecification(slotIds));



                    if (slots is null || !slots.Any() ) return Result.Fail("Slots Not Avaialble");

                    //var reservations = await reservationRepo.GetAllAsync(
                    //    new GetReservationsBySlotIdsSpecification(slotIds));


                    var tourDict = tours.ToDictionary(x => x.Id);
                    var userType = await GetUserType(userId);

                    var priceDict = tours
                        .SelectMany(t => t.Prices
                            .Where(p => p.UserType == userType)
                            .SelectMany(p => p.Translations
                                .Select(tr => new
                                {
                                    Key = (
                                        t.Id,
                                        tr.Type.ToLower().Trim(),
                                        tr.ActivityType?.ToLower().Trim(),
                                        tr.Language
                                    ),
                                    Price = p
                                })))
                        .ToDictionary(
                            x => x.Key,
                            x => x.Price);

                    //create slots dictionary



                    var slotDict = slots.ToDictionary(x => x.Id);

                    //create reservations dictionary



                   

                    var newReservations = new List<SlotReservation>(cart.Items.Count);

                    // handle Travelers

                    var requestItemMap = requestDTO.Items
                            .ToDictionary(x => x.CartItemId);

                    var slotRequests = cart.Items
                        .GroupBy(x => x.SlotId)
                        .Select(g => new SlotReservationRequest(
                            g.Key,
                            g.Sum(item =>
                                item.Prices.Sum(p => p.Quantity))))
                        .ToList();

                    var reservedIds = await _slotsRepo.ReserveBulkAsync(slotRequests);

                    if (reservedIds.Count != slotRequests.Count)
                    {
                        await transaction.RollbackAsync();

                        return Result.Fail(
                            "One or more slots are full.");
                    }

                    var rate = await _currencyRateService.GetRateAsync(
                       Constants.BaseCurrency,
                       cart.CurrencyCode.Value, false);

                    if (!rate.IsSuccess)
                        return Result.Fail(rate.Errors);
                    var exchangeRate = rate.ValueOrDefault;

                    foreach (var item in cart.Items.Where(i => !i.IsDeleted))
                    {
                        if (!tourDict.TryGetValue(item.TourId, out var tour))
                            return Result.Fail($"Tour {item.TourId} not found");

                        if (!slotDict.TryGetValue(item.SlotId, out var orderedSlot))
                            return Result.Fail("Slot not found");
                        

                        if (!requestItemMap.TryGetValue(item.Id, out var itemRequest))
                            return Result.Fail("Missing item Travelers request");


                        


                        var prices = item.Prices;
                        var totalPeople = prices.Sum(x => x.Quantity);

                        if (itemRequest.Travelers.Count != totalPeople)
                        {
                            return Result.Fail($"You Should Enter {totalPeople} Passanger Details");

                        }



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
                            ActivityType = item.ActivityType,
                            //CurrencyCode = cart.CurrencyCode,
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
                                tour.Cancellation.RefundPercentage,
                            TravelersDraft = BuildTravelers(itemRequest)
                        };

                        decimal itemTotal = 0;
                       


                        foreach (var cartPrice in prices)
                        {
                            var key = (
                                        tour.Id,
                                        cartPrice.Type.ToLower().Trim(),
                                        item.ActivityType?.ToLower().Trim(),
                                        item.Language
                                    );

                            var found = priceDict.TryGetValue(key, out var priceEntity);

                            if (priceEntity is null)
                                return Result.Fail(
                                    $"Price type {cartPrice.Type} not found for tour {tour.Id}");

                            var currentRetailPrice = priceEntity.RetailPrice;

                            if (cartPrice.BaseRetailPrice != currentRetailPrice)
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
                                BaseRetailPrice = currentRetailPrice,
                                ExchangeRate = exchangeRate,
                                ConvertedRetailPrice = exchangeRate * currentRetailPrice,
                            });

                            itemTotal += exchangeRate * currentRetailPrice * cartPrice.Quantity;
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
                        Currency = cart.CurrencyCode.GetValueOrDefault()
                    };

                    await _unitOfWork
                        .GetRepository<Payment, Guid>()
                        .AddAsync(payment);


                   

                 

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                    // remove cart from data base
                    await _cacheService.RemoveAsync(
                                    BuildCartCacheKey(userId, cartToken));
                    return Result.Ok(
                        response with
                        {
                            PaymentId = payment.Id,
                            CurrencyCode = payment.Currency.ToString()
                           
                        });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Checkout Failed: {ex.Message}");
                    return FluentValidationExtension
                        .FromException(details: ex.Message);
                }
            });
        }

        private List<TravelerDraft> BuildTravelers(
            CheckoutItemRequestDTO requestItem)
        {
            return requestItem.Travelers.Select(t => 
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

                return new TravelerDraft
                {
                    Id = Guid.NewGuid(),
                    FullName = $"{t.FirstName} {t.LastName}",
                    Nationality = t.Nationality,
                    Type = t.Type,
                    BirthDate = t.BirthDate,
                    PassportNumber = finalPassport,
                    CartItemId = requestItem.CartItemId
                };
            }).ToList();
        }


        private void RecalculateCart(Cart cart)
        {

            cart.TotalAmount = cart.Items.Sum(x => x.TotalAmount);
            cart.LastUpdatedAt = DateTime.UtcNow;
        }
        private decimal GetPriceFromTour(IEnumerable<Price> prices , string type, SupportedLanguage lang,string? activityType)
        {
            var price = prices
                .FirstOrDefault( p =>
                    p.Translations.Any(t =>
                        t.Type == type &&
                        (string.IsNullOrWhiteSpace(activityType) || t.ActivityType == activityType ) &&
                        t.Language == lang));
            
            if (price == null)
                return 0;

            return price.RetailPrice;
        }
        private async Task<Cart?> GetOrCreateCart(string? userId, string? cartToken, bool autoCreate = true)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
            Cart? userCart = null;
            Cart? tokenCart = null;

            if (!string.IsNullOrWhiteSpace(userId))
                userCart = await cartRepo.GetByIdAsync(new GetCartWithUserIdSpecification(userId));

            if (!string.IsNullOrWhiteSpace(cartToken))
                tokenCart = await cartRepo.GetByIdAsync(new GetCartWithCartTokenSpecification(cartToken));

            if (userCart != null && tokenCart != null && userCart.Id != tokenCart.Id)
            {
                if (tokenCart.Items.Any())
                {
                    foreach (var item in tokenCart.Items.ToList())
                    {
                        item.CartId = userCart.Id;
                        item.Cart = userCart;
                        userCart.Items.Add(item);
                    }
                    RecalculateCart(userCart);
                }
                
                cartRepo.Remove(tokenCart);
                await _unitOfWork.SaveChangesAsync();
                return userCart;
            }

            if (userCart == null && tokenCart != null && !string.IsNullOrWhiteSpace(userId))
            {
                tokenCart.UserId = userId;
                await _unitOfWork.SaveChangesAsync();
                return tokenCart;
            }

            var cart = userCart ?? tokenCart;
            if (cart is not null)
                return cart;

            if (!autoCreate)
                return null;
           
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CartToken = cartToken ?? Guid.NewGuid().ToString(),
                CurrencyCode = null,
                LastUpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(15)
            };
            await cartRepo.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            
            return cart;
        }

        public async Task<Result<string>> RemoveItemAsync(
                Guid itemId,
                string? userId,
                string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
            if (cart == null) return Result.Ok("Cart already empty");

            var item = cart.Items.FirstOrDefault(x => x.Id == itemId);

            if (item is null)
                return Result.Fail(new NotFoundError("Cart item not found"));

            cart.Items.Remove(item);

            RecalculateCart(cart);

            await _unitOfWork.SaveChangesAsync();

            // UPDATE CACHE
            var cacheKey =
                 BuildCartCacheKey(userId, cartToken);

            if (!cart.Items.Any())
            {
                _unitOfWork.GetRepository<Cart, Guid>().Remove(cart);
                await _cacheService.RemoveAsync(cacheKey);
            }
            else
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    cart.ToDto(),
                    TimeSpan.FromMinutes(25));
            }

            return Result.Ok("Cart Item Deleted Successfully");
        }
        public async Task<Result<string>> ClearAsync(
                string? userId,
                string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);

            if (cart != null && cart.Items.Any())
            { 
                cart.Items.Clear();
                 _unitOfWork.GetRepository<Cart, Guid>().Remove(cart);
            }



            //cart.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            var cacheKey =
                 BuildCartCacheKey(userId, cartToken);
            await _cacheService.RemoveAsync(cacheKey);

            return Result.Ok("Cart Deleted Successfully");
        }


        private string BuildCartCacheKey(
            string? userId,
            string? cartToken)
        {
            if (!string.IsNullOrWhiteSpace(userId))
                return $"cart:user:{userId}";

            return $"cart:guest:{cartToken}";
        }
    }
}
