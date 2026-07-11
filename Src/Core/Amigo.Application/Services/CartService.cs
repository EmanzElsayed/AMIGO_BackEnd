using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BlackoutDateSpecification;
using Amigo.Application.Specifications.BlackoutWeekDaysSpecification;
using Amigo.Application.Specifications.CancellationSpecification;
using Amigo.Application.Specifications.CartSpecification;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.Application.Specifications.TourSpecification.User;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.Cart;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using PhoneNumbers;
using System.Collections.Frozen;


namespace Amigo.Application.Services
{
    public class CartService(IUnitOfWork _unitOfWork ,
        EncryptionService _encryptionService,
        ICurrencyRateService _currencyRateService,
        IUserRepo _userRepo,
        ICacheService _cacheService,
        IValidationService _validationService,

        Microsoft.AspNetCore.Identity.UserManager<Amigo.Domain.Entities.Identity.ApplicationUser> _userManager
        ) 
        : ICartService
    {
        private readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

        public async Task<Result<CartDTO>> GetCurrentCartAsync(string? userId, string? cartToken)
            {
                var cacheKey =
                BuildCartCacheKey(userId, cartToken);

                // 1. Try Cache
                var cached =
                    await _cacheService
                        .GetAsync<CartDTO>(cacheKey);
              
                if (cached is not null && cached.Items.Any())
                    return Result.Ok(cached);

            // 2. DB
               
                var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
                
            ApplicationUser? user = !string.IsNullOrWhiteSpace(userId) ? await _userRepo.GetByIdWithoutSpecAsync(userId) : null;
                
                PhoneNumber? number = user is not null && !string.IsNullOrWhiteSpace(user.PhoneNumber) ?  _phoneUtil.Parse(user.PhoneNumber, null) : null ;
            //if( )  number = 
            string? nationality = user is null ? null : (string.IsNullOrWhiteSpace(user.Nationality) ? null : user.Nationality);
            string? countryCode = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) ? null : _phoneUtil.GetRegionCodeForNumber(number));
            string? phoneNumber = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) || number == null ? null : number.NationalNumber.ToString());

            if (cart == null || !cart.Items.Any())
                {
                    return Result.Ok(new CartDTO(
                        Guid.Empty,
                        userId,
                       nationality,
                       countryCode,
                        phoneNumber,

                        cartToken,
                        null,
                        0,
                        0,
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddDays(15),
                        new List<CartItemDTO>()
                    ));
                }

                var tourIds = cart.Items.Select(i => i.TourId);
                var imageDic = await _unitOfWork.TourRepo.GetFirstTourImagesAsync(tourIds);
            
                var MappedCart = cart.ToDto(imageDic,_encryptionService,nationality,countryCode,phoneNumber);
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


            var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(new GetTourByIdSpecification(requestDTO.TourId));
            if (tour is null)
            {
                return Result.Fail(new NotFoundError("This Tour Not Found"));

            }
            

            var blackoutDates = (await _unitOfWork.GetRepository<BlackoutDate, Guid>().GetAllAsync(new GetBlackoutDatesWithTourIdSpecification(tour.Id))).Select(b => b.Date).ToList();
            if (blackoutDates.Contains(requestDTO.TourDate))
            {
                return Result.Fail(new NotFoundError("This Date Not Allowed"));

            }
            var blackoutWeekDays = (await _unitOfWork.GetRepository<BlackoutWeekDay, Guid>().GetAllAsync(new GetBlackoutWeekDayesSpecification(tour.Id))).Select(b => b.DayOfWeek).ToList();
            if (blackoutWeekDays.Contains(requestDTO.TourDate.DayOfWeek))
            {
                return Result.Fail(new NotFoundError("This Day Not Allowed"));

            }
            AvailableSlots? slot = null; 
            if (tour.IsFullTime == false)
            { 
                var availableSlots = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetAllAsync(new GetAllSlotsWithTourIdSpecification(tour.Id));
                if (availableSlots is null || !availableSlots.Any())
                {
                    return Result.Fail(new NotFoundError("This Tour does not has available time "));

                }

               slot = availableSlots.Where(a => a.StartTime == requestDTO.StartTime && a.AvailableTimeStatus == AvailableDateTimeStatus.Available).FirstOrDefault();

                if (slot is null)
                {
                    return Result.Fail("This Slot Not Available Now");

                }

            }


            var requestedQty = requestDTO.Prices.Sum(x => x.Quantity);
            
            var translatedTitle = tour.Translations
                   .FirstOrDefault(x => x.Language == requestDTO.Language)?.Title
                   ?? tour.Translations.FirstOrDefault()?.Title
                   ?? "Tour";
            var destinationName = tour.Destination?.Translations?
            .FirstOrDefault(t => t.Language == requestDTO.Language)?.Name
            ?? tour.Destination?.Translations?.FirstOrDefault()?.Name
            ?? string.Empty;

            string? activityType = requestDTO.Prices
                                    .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ActivityType))?.ActivityType ?? null;

            var item = new CartItem
            {

                Cart = cart,
                CartId = cart.Id,
                TourId = requestDTO.TourId,
                Tour = tour,
                SlotId = slot?.Id ?? null,
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
                    cart.CurrencyCode.Value, false);

            if (!rate.IsSuccess)
                return Result.Fail(rate.Errors);


            var prices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PricesForTourSpecification(tour.Id, userType));
            
            var specialDates = prices.Select(p => p.SpecialDate).Distinct().ToList();
            bool isSpecialDate = false;
            if (specialDates.Contains(requestDTO.TourDate)) isSpecialDate = true;

            

            foreach (var p in requestDTO.Prices)
            {

                var retailPrice = GetPriceFromTour(prices, p.Type, requestDTO.Language, activityType,requestDTO.TourDate,isSpecialDate);

                item.Prices.Add(new CartPrice
                {

                    Type = p.Type,
                    Quantity = p.Quantity,
                    BaseRetailPrice = retailPrice,
                    ConvertedRetailPrice = retailPrice * rate.ValueOrDefault,
                    ExchangeRate = rate.ValueOrDefault

                });
            }
            item.IsSpecialDate = isSpecialDate;
            item.ActivityType = activityType;
            item.TotalAmount = item.Prices.Sum(x => x.FinalPrice);

            cart.Items.Add(item);

            RecalculateCart(cart);

            await _unitOfWork.SaveChangesAsync();

            var tourIds = cart.Items.Select(i => i.TourId);
            var imageDic = await _unitOfWork.TourRepo.GetFirstTourImagesAsync(tourIds);
            ApplicationUser? user = !string.IsNullOrWhiteSpace(userId) ? await _userRepo.GetByIdWithoutSpecAsync(userId) : null;

            PhoneNumber? number = user is not null && !string.IsNullOrWhiteSpace(user.PhoneNumber) ? _phoneUtil.Parse(user.PhoneNumber, null) : null;
            //if( )  number = 
            string? nationality = user is null ? null : (string.IsNullOrWhiteSpace(user.Nationality) ? null : user.Nationality);
            string? countryCode = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) ? null : _phoneUtil.GetRegionCodeForNumber(number));
            string? phoneNumber = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) || number == null ? null : number.NationalNumber.ToString());

            var dto = cart.ToDto(imageDic, _encryptionService,nationality,countryCode,phoneNumber);
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

        private string FormatPhone(string phone, string region)
        {
            var number = _phoneUtil.Parse(phone, region);
            return _phoneUtil.Format(number, PhoneNumberFormat.E164);
        }
        public async Task<Result<CartDTO>> UpdateItemAsync(
                Guid itemId,
                string? userId,
                string? cartToken,
                UpdateCartItemRequestDTO dto)
        {

            var validationResult = await _validationService.ValidateAsync(dto);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var cart = await GetOrCreateCart(userId, cartToken, autoCreate: false);
            if (cart == null) return Result.Fail(new NotFoundError("Cart not found"));

            ApplicationUser? user = string.IsNullOrWhiteSpace(userId) ? null : await _userRepo.GetByIdWithoutSpecAsync(userId);
            if (user is not null)
            {
                user.Nationality = string.IsNullOrWhiteSpace( dto.Nationality) ? null : dto.Nationality;
                user.PhoneNumber = !string.IsNullOrWhiteSpace(dto.PhoneNumber) && !string.IsNullOrWhiteSpace(dto.CountryIsoCode) ? FormatPhone(dto.PhoneNumber, dto.CountryIsoCode) : null;


            }
            var item = cart.Items.FirstOrDefault(x => x.Id == itemId);

            if (item is null || item.CartId != cart.Id)
                return Result.Fail(new NotFoundError("This Item Not Found"));


            if (dto.Prices != null && dto.Prices.Any())
            {
                var requestedQty = dto.Prices.Sum(x => x.Quantity);
                var currentItemQty = item.Prices.Sum(x => x.Quantity);

                // We check if the INCREASE exceeds the remaining capacity
                // Slot is already attached to item


                //var slot = await _unitOfWork.GetRepository<AvailableSlots, Guid>().GetByIdAsync(item.SlotId);
                //if (slot is null)
                //    return Result.Fail(new NotFoundError("This Slot Not Found"));

                //var remaining = slot.MaxCapacity - slot.ReservedCount;

                //if (requestedQty > currentItemQty && (requestedQty - currentItemQty) > remaining)
                //{
                //    return Result.Fail($"Sorry, only {remaining + currentItemQty} total spots are available.");
                //}

                var priceRepo = _unitOfWork.GetRepository<CartPrice, Guid>();
                var cartPrices = await priceRepo.GetAllAsync(new GetCartPriceWithCartItemIdSpecification(itemId));
                priceRepo.RemoveRange(cartPrices);



                var userType = await GetUserType(userId);

                var rate = await _currencyRateService.GetRateAsync(
                Constants.BaseCurrency,
                cart.CurrencyCode.Value, false);

                if (!rate.IsSuccess)
                    return Result.Fail(rate.Errors);

                string? activityType = dto.Prices
                                                  .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ActivityType))?.ActivityType ?? null;

                var prices = await _unitOfWork.GetRepository<Price, Guid>().GetAllAsync(new PricesForTourSpecification(item.TourId, userType));
                
                bool isSpecialDate = item.IsSpecialDate;

                List<CartPrice> newCartprices = new List<CartPrice>();

                foreach (var p in dto.Prices)
                {
                    var retailPrice = GetPriceFromTour(prices, p.Type, item.Language, activityType,item.TourDate,isSpecialDate);

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
                        Nationality = t.Nationality ?? null,
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

            var tourIds = cart.Items.Select(i => i.TourId);
            var imageDic = await _unitOfWork.TourRepo.GetFirstTourImagesAsync(tourIds);

            PhoneNumber? number = user is not null && !string.IsNullOrWhiteSpace(user.PhoneNumber) ? _phoneUtil.Parse(user.PhoneNumber, null) : null;
            //if( )  number = 
            string? nationality = user is null ? null : (string.IsNullOrWhiteSpace(user.Nationality) ? null : user.Nationality);
            string? countryCode = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) ? null : _phoneUtil.GetRegionCodeForNumber(number));
            string? phoneNumber = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) || number == null ? null : number.NationalNumber.ToString());

            var mappedCart = cart.ToDto(imageDic, _encryptionService,nationality,countryCode,phoneNumber);
            await _cacheService.SetAsync(
            BuildCartCacheKey(userId, cartToken),
            dto,
            TimeSpan.FromMinutes(25));

            return mappedCart;
        }




        public async Task<Result<CheckoutResponseDTO>> CheckoutAsync(CheckoutRequestDTO requestDTO, string userId, string? cartToken)
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
                    decimal totalWithUsd = 0;

                    var tourRepo = _unitOfWork.GetRepository<Tour, Guid>();

                    var slotRepo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

                    var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();


                    // GEt Ids

                    var tourIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.TourId).ToHashSet();
                    //var slotIds = cart.Items.Where(i => !i.IsDeleted).Select(x => x.SlotId).Distinct().OrderBy(x => x).ToList();
                    var orderId = order.Id;

                    //create tour dictionary

                    var tours = await tourRepo.GetAllAsync(
                                     new GetToursByIdsSpecification(tourIds));

                    var cancellations = await _unitOfWork.GetRepository<Cancellation, Guid>().GetAllAsync(new GetAllCancellationWithToursIdsSpecification(tourIds));
                    var cancellationDic = cancellations.GroupBy(c => c.TourId)
                                        .ToDictionary(g => g.Key,
                                                      g => g.ToList());


                    var blackoutDates = await _unitOfWork.GetRepository<BlackoutDate, Guid>().GetAllAsync(new GetBlackoutDatesWithTourIdsSpecification(tourIds));
                    var blackoutDatesDic = blackoutDates
                        .GroupBy(b => b.TourId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.Date).ToList()
                        );
                    var blackoutWeekDays = await _unitOfWork.GetRepository<BlackoutWeekDay, Guid>().GetAllAsync(new GetBlackoutWeekDaysWithTourIdsSpecification(tourIds));
                    var blackoutweekDaysDic = blackoutWeekDays.GroupBy(w => w.TourId)

                                                            .ToDictionary(
                                                            g => g.Key,
                                                            g => g.Select(x => x.DayOfWeek).ToList());

                    var availableTimes = await slotRepo.GetAllAsync(new GetAvailableTimesWithTourIdsSpecification(tourIds));
                    var availableTimesDic = availableTimes.GroupBy(w => w.TourId)

                                                            .ToDictionary(
                                                            g => g.Key,
                                                            g => g.Select(x => x.StartTime).ToList()); 


                    //var slots = await slotRepo.GetAllAsync(
                    //         new GetSlotsByIdsSpecification(slotIds));



                    //if (slots is null || !slots.Any()) return Result.Fail("Slots Not Avaialble");

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
                                        tr.Language,
                                        p.SpecialDate
                                    ),
                                    Price = p
                                })))
                        .ToDictionary(
                            x => x.Key,
                            x => x.Price);

                    //create slots dictionary



                    //var slotDict = slots.ToDictionary(x => x.Id);

                    //create reservations dictionary





                    var newReservations = new List<SlotReservation>(cart.Items.Count);

                    // handle Travelers

                    var requestItemMap = requestDTO.Items
                            .ToDictionary(x => x.CartItemId);

                    //var slotRequests = cart.Items
                    //    .GroupBy(x => x.SlotId)
                    //    .Select(g => new SlotReservationRequest(
                    //        g.Key,
                    //        g.Sum(item =>
                    //            item.Prices.Sum(p => p.Quantity))))
                    //    .ToList();

                    //var reservedIds = await _slotsRepo.ReserveBulkAsync(slotRequests);

                    //if (reservedIds.Count != slotRequests.Count)
                    //{
                    //    await transaction.RollbackAsync();

                    //    return Result.Fail(
                    //        "One or more slots are full.");
                    //}

                    var rate = await _currencyRateService.GetRateAsync(
                       Constants.BaseCurrency,
                       cart.CurrencyCode.Value, false);

                    if (!rate.IsSuccess)
                        return Result.Fail(rate.Errors);
                    var exchangeRate = rate.ValueOrDefault;

                    foreach (var item in cart.Items.Where(i => !i.IsDeleted))
                    {

                        if (!tourDict.TryGetValue(item.TourId, out var tour))
                            return Result.Fail($"Tour {item.TourTitle} not found");
                        if (item.TourDate.ToDateTime(item.StartTime) <= DateTime.UtcNow)
                        {
                            return Result.Fail($"Tour {item.TourTitle} up to date");

                        }
                        blackoutDatesDic.TryGetValue(item.TourId, out var blackoutDatesForitem);
                        if( blackoutDatesForitem is not null && blackoutDatesForitem.Any() && blackoutDatesForitem.Contains(item.TourDate))
                            return Result.Fail("Date not allowed");
                        
                        blackoutweekDaysDic.TryGetValue(item.TourId, out var blackoutWeekdaysForitem);
                        if (blackoutWeekdaysForitem is not null && blackoutWeekdaysForitem.Any() && blackoutWeekdaysForitem.Contains(item.TourDate.DayOfWeek))
                            return Result.Fail("Date not allowed");

                        if (tour.IsFullTime == false)
                        {
                            availableTimesDic.TryGetValue(item.TourId, out var availableTimesForitem);
                            if (availableTimesForitem is not null && availableTimesForitem.Any() && !availableTimesForitem.Contains(item.StartTime))
                                return Result.Fail("Time not allowed");

                        }


                        //if (!slotDict.TryGetValue(item.SlotId, out var orderedSlot))
                        //    return Result.Fail("Slot not found");


                        //if (!requestItemMap.TryGetValue(item.Id, out var itemRequest))
                        //    return Result.Fail("Missing item Travelers request");





                        var prices = item.Prices;
                        var totalPeople = prices.Sum(x => x.Quantity);

                        //if (itemRequest.Travelers.Count != totalPeople)
                        //{
                        //    return Result.Fail($"You Should Enter {totalPeople} Passanger Details");

                        //}



                        // reserved  slot 
                        var reservation = new SlotReservation
                        {
                            Id = Guid.NewGuid(),
                            SlotId = item.SlotId,
                            Slot = item.Slot,
                            TourDateTime = DateTime.SpecifyKind(
                                        item.TourDate.ToDateTime(item.StartTime),
                                        DateTimeKind.Utc
                                    ),
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
                        requestItemMap.TryGetValue(item.Id, out var itemRequest);
                        cancellationDic.TryGetValue(item.TourId, out var cancellationsPolicy);

                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            TourId = tour.Id,
                            SlotId = item.SlotId,
                            TourDate = item.TourDate,
                            StartTime = item.StartTime,
                            ActivityType = item.ActivityType,
                            IsSpecialDate = item.IsSpecialDate,
                            //CurrencyCode = cart.CurrencyCode,
                            Language = item.Language,
                            MeetingPoint = tour.MeetingPoint,
                            Duration = tour.Duration,
                            TourTitle = tourTitle,
                            DestinationName = destinationName,
                            CancellationPolicies = cancellationsPolicy is null || !cancellationsPolicy.Any() ? null:
                                                     cancellationsPolicy
                                                    .Select(x => new OrderItemCancellationPolicy
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        CancelationPolicyType = x.CancelationPolicyType,
                                                        CancellationBefore = x.CancellationBefore,
                                                        RefundPercentage = x.RefundPercentage
                                                    }).ToList()

                            ,
                            TravelersDraft =itemRequest != null && itemRequest.Travelers != null && itemRequest.Travelers.Any() ? BuildTravelers(itemRequest) : null
                        };

                        decimal itemTotal = 0;
                        decimal itemTotalWithUsd = 0;
                        

                        foreach (var cartPrice in prices)
                        {
                            var key = (
                                        tour.Id,
                                        cartPrice.Type.ToLower().Trim(),
                                        item.ActivityType?.ToLower().Trim(),
                                        item.Language,
                                        item.IsSpecialDate ? item.TourDate : (DateOnly?)null
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
                            itemTotalWithUsd += currentRetailPrice * cartPrice.Quantity;
                        }

                        total += itemTotal;
                        totalWithUsd += itemTotalWithUsd;
                        order.OrderItems.Add(orderItem);
                    }

                    order.TotalAmount = total;
                    order.TotalAmountWithUsd = totalWithUsd;
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
                        TotalAmountWithUsd = totalWithUsd,
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

                string? finalPassport = null;
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
        private decimal GetPriceFromTour(IEnumerable<Price> prices , string type, SupportedLanguage lang,string? activityType,DateOnly date , bool isSpecialDate)
        {
            var price = prices
                .FirstOrDefault( p =>
                    ((isSpecialDate == false && p.SpecialDate == null) || (isSpecialDate && p.SpecialDate == date) )
                    &&
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
                var tourIds = cart.Items.Select(i => i.TourId);
                var imageDic = await _unitOfWork.TourRepo.GetFirstTourImagesAsync(tourIds);

                ApplicationUser? user = !string.IsNullOrWhiteSpace(userId) ? await _userRepo.GetByIdWithoutSpecAsync(userId) : null;

                PhoneNumber? number = user is not null && !string.IsNullOrWhiteSpace(user.PhoneNumber) ? _phoneUtil.Parse(user.PhoneNumber, null) : null;
                //if( )  number = 
                string? nationality = user is null ? null : (string.IsNullOrWhiteSpace(user.Nationality) ? null : user.Nationality);
                string? countryCode = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) ? null : _phoneUtil.GetRegionCodeForNumber(number));
                string? phoneNumber = user is null ? null : (string.IsNullOrWhiteSpace(user.PhoneNumber) || number == null ? null : number.NationalNumber.ToString());

                await _cacheService.SetAsync(
                    cacheKey,
                    cart.ToDto(imageDic, _encryptionService,nationality,countryCode,phoneNumber),
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
