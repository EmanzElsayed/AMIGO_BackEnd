using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.Cart;


namespace Amigo.Application.Services
{
    public class CartService(IUnitOfWork _unitOfWork ,
        EncryptionService _encryptionService,
        IPaymentOrchestrator _paymentOrchestrator,
        ISlotsRepo _slotsRepo,
        Microsoft.AspNetCore.Identity.UserManager<Amigo.Domain.Entities.Identity.ApplicationUser> _userManager
        ) 
        : ICartService
    {
        public async Task<Result<CartDTO>> GetCurrentCartAsync(string? userId, string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);

            if (cart == null)
            {
                return Result.Ok(new CartDTO(
                    Guid.Empty,
                    userId,
                    cartToken,
                    "USD",
                    0,
                    0,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(15),
                    new List<CartItemDTO>()
                ));
            }

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
            if (slot is null)
            {
                return Result.Fail(new NotFoundError("This Slot Not Found"));

            }
            if (slot.AvailableTimeStatus != AvailableDateTimeStatus.Available)
            {
                return Result.Fail("This Slot Not Available Now");

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

            var userType = await GetUserType(userId);

            foreach (var p in requestDTO.Prices)
            {
                var retailPrice = GetPriceFromTour(tour, p.Type,requestDTO.Language,userType);

                item.Prices.Add(new CartPrice
                {
                    
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



        private async Task<UserType> GetUserType(string? userId)
        {


            if (userId is null) return UserType.Public;
            var user = await _userManager.FindByIdAsync(userId);

            string role = "Customer";

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
                var priceRepo = _unitOfWork.GetRepository<CartPrice, Guid>();

                item.Prices.Clear();

                var userType = await GetUserType(userId);

                foreach (var p in dto.Prices)
                {
                    var retailPrice = GetPriceFromTour(item.Tour, p.Type, item.Language, userType);

                    item.Prices.Add(new CartPrice
                    {
                        Id = Guid.NewGuid(),
                        CartItemId = item.Id,
                        Type = p.Type,
                        Quantity = p.Quantity,
                        RetailPrice = retailPrice
                    });
                }

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

            if (dto.PhoneCode != null || dto.PhoneNumber != null || dto.HotelNameAddress != null || dto.CommentForProvider != null)
            {
                item.PhoneCode = dto.PhoneCode;
                item.PhoneNumber = dto.PhoneNumber;
                item.HotelNameAddress = dto.HotelNameAddress;
                item.CommentForProvider = dto.CommentForProvider;
            }

            item.TotalAmount = item.Prices.Sum(x => x.FinalPrice);

            RecalculateCart(cart);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error($"DB Error: {ex.Message} Inner: {ex.InnerException?.Message}"));
            }

            return cart.ToDto();
        }


        public async Task<Result<CartItemBookingDetailDTO>> GetBookingDetailAsync(Guid itemId, string? userId, string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
            if (cart == null) return Result.Fail(new NotFoundError("Cart not found"));
            
            var itemRepo = _unitOfWork.GetRepository<CartItem, Guid>();
            var item = await itemRepo.GetByIdAsync(new GetCartItemWithIdSpecification(itemId));

            if (item is null || item.CartId != cart.Id)
                return Result.Fail(new NotFoundError("This Item Not Found"));

            var travelers = item.Travelers?.Select(t => new CheckoutTravelersRequestDTO
            (
                Type: t.Type ?? "",
                FirstName: t.FullName.Split(' ', 2).FirstOrDefault() ?? "",
                LastName: t.FullName.Split(' ', 2).LastOrDefault() ?? "",
                Nationality: t.Nationality,
                PassportNumber: string.IsNullOrWhiteSpace(t.PassportNumber) ? "" : _encryptionService.Decrypt(t.PassportNumber),
                BirthDate: t.BirthDate ?? DateOnly.FromDateTime(DateTime.Today)
            )).ToList() ?? new List<CheckoutTravelersRequestDTO>();

            string? firstName = null;
            string? lastName = null;
            string? email = null;
            string? phoneNumber = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    firstName = user.FullName?.Split(' ', 2).FirstOrDefault();
                    lastName = user.FullName?.Split(' ', 2).LastOrDefault();
                    email = user.Email;
                    phoneNumber = user.PhoneNumber;
                }
            }

            return Result.Ok(new CartItemBookingDetailDTO(
                travelers, 
                firstName, 
                lastName, 
                email, 
                item.PhoneCode,
                item.PhoneNumber ?? phoneNumber, 
                item.HotelNameAddress, 
                item.CommentForProvider));
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

                    var tourIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.TourId).Distinct().ToList();
                    var slotIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.SlotId).Distinct().OrderBy(x => x).ToList();
                    var orderId = order.Id;

                    //create tour dictionary

                    var tours =  await tourRepo.GetAllAsync(
                                     new GetToursByIdsSpecification(tourIds));



                    var slots = await slotRepo.GetAllAsync(
                             new GetSlotsByIdsSpecification(slotIds));



                    if (slots is null || !slots.Any() ) return Result.Fail("Slots Not Avaialble");

                    var reservations = await reservationRepo.GetAllAsync(
                        new GetReservationsBySlotIdsSpecification(slotIds));


                    var tourDict = tours.ToDictionary(x => x.Id);
                    var userType = await GetUserType(userId);

                    var priceDict = tours
                          .SelectMany(t => t.Prices
                              .Where(p => p.UserType == userType)
                              .SelectMany(p => p.Translations.Select(tr => new
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
                                tour.Cancellation.RefundPercentage,
                            NameAndAddressOfAccomodation = itemRequest.NameAndAddressAccommodation,
                            CommentForProvider = itemRequest.CommentForProvider,
                            PhoneCode = itemRequest.PhoneCode,
                            PhoneNumber = itemRequest.PhoneNumber,

                            TravelersDraft = BuildTravelers(itemRequest)
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
        private decimal GetPriceFromTour(Tour tour, string type, Language lang,UserType userType)
        {
            var price = tour.Prices
                .FirstOrDefault(p => p.UserType == userType &&
                    p.Translations.Any(t =>
                        t.Type == type &&
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
                CurrencyCode = CurrencyCode.USD,
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

            return Result.Ok("Cart Item Deleted Successfully");
        }
        public async Task<Result<string>> ClearAsync(
                string? userId,
                string? cartToken)
        {
            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);

            if (cart != null && cart.Items.Any())
                cart.Items.Clear();

            cart.TotalAmount = 0;
            cart.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result.Ok("Cart Deleted Successfully");
        }
    }
}
