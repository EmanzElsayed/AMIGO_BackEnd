using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.UserSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Domain.DTO.User;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Amigo.Application.Specifications.TourSpecification;
using Amigo.SharedKernal.QueryParams;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminCustomerService(
                    IUserRepo _userRepo,
                    IUnitOfWork _unitOfWork,
                    UserManager<ApplicationUser> _userManager)

                 : IAdminCustomerService
    {
        public async Task<Result<GetCustomersDashboardResponseDTO>> GetadminCustomerDashboardAsync()
        {
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            // Get all necessary data
            var adminIds = await _userRepo.GetUserIdsInRoleAsync("Admin");
            var vipIds = await _userRepo.GetUserIdsInRoleAsync("VIP");

            var allBookings = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetAllBookingSpecification());
            var allPayments = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(new GetAllSucceedPaymentSpecification());
            var allUsers = await _userRepo.GetAllAsync(new GetAllUserSpecification());

            var currentMonthBookings = allBookings.Where(b =>
                b.CreatedDate >= currentMonthStart && b.CreatedDate < nextMonthStart).ToList();

            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthBookings = allBookings.Where(b =>
                b.CreatedDate >= previousMonthStart && b.CreatedDate < currentMonthStart).ToList();

            var currentMonthPayments = allPayments.Where(p =>
                p.CreatedDate >= currentMonthStart && p.CreatedDate < nextMonthStart).ToList();

            int totalCustomers = allUsers.Count(u => !adminIds.Contains(u.Id));
            int vipMembers = vipIds.Count();
            int bookingsThisMonth = currentMonthBookings.Count();
            decimal grossRevenue = currentMonthPayments.Sum(p => p.TotalAmount);

            string monthlyGrowthText = await ComputeMonthlyGrowth(adminIds);

            var dailyRevenue = CalculateDailyRevenue(currentMonthPayments);

            var regionalMix = CalculateRegionalMix(currentMonthBookings);

            var initialQuery = new GetAllAdminTourQuery { PageNumber = 1, PageSize = 5 };
            var topActivitiesResult = await GetDashboardActivitiesAsync(initialQuery);
            var topActivities = topActivitiesResult.IsSuccess ? topActivitiesResult.Value.Data.ToList() : new List<TopPerformingActivityDTO>();

            return Result.Ok(new GetCustomersDashboardResponseDTO
            {
                BookingsThisMonth = bookingsThisMonth,
                GrossRevenue = grossRevenue,
                TotalCustomers = totalCustomers,
                VipMembers = vipMembers,
                MonthlyGrowthText = monthlyGrowthText,
                DailyRevenue = dailyRevenue,
                RegionalMix = regionalMix,
                TopPerformingActivities = topActivities
            });
        }

        public async Task<Result<PaginatedResponse<TopPerformingActivityDTO>>> GetDashboardActivitiesAsync(GetAllAdminTourQuery query)
        {
            query.FilterActiveOnly = true;
            var now = DateTime.UtcNow;
            var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            var nextDayStart = todayStart.AddDays(1);
            var previousDayStart = todayStart.AddDays(-1);

            var spec = new GetAllToursForAdminSpecification(query);
            var tours = await _unitOfWork.GetRepository<Tour, Guid>().GetAllAsync(spec);
            var totalCount = await _unitOfWork.GetRepository<Tour, Guid>().GetCountSpecificationAsync(new CountAllToursForAdminSpecification(query));

            var allBookings = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetAllBookingSpecification());
            
            var bookingsPerTour = allBookings
                            .Where(b => b.OrderItem != null &&
                                        b.OrderItem.TourId.HasValue)
                            .GroupBy(b => b.OrderItem!.TourId!.Value)
                            .ToDictionary(g => g.Key, g => g.Count());

            var maxBookings = bookingsPerTour.Any()
                ? bookingsPerTour.Max(x => x.Value)
                : 0;

            var items = tours.Select(t =>
            {
                var translation = t.Translations.FirstOrDefault(tr => tr.Language.ToString().Equals(query.Language, StringComparison.OrdinalIgnoreCase))
                                 ?? t.Translations.FirstOrDefault()
                                 ?? new TourTranslation { Title = "Unknown" };

                var destTranslation = t.Destination?.Translations.FirstOrDefault(tr => tr.Language.ToString().Equals(query.Language, StringComparison.OrdinalIgnoreCase))
                                     ?? t.Destination?.Translations.FirstOrDefault()
                                     ?? new DestinationTranslation { Name = "Unknown" };

                var tourBookings =  allBookings.Where(b => b.OrderItem?.TourId == t.Id).ToList();
                var currentDayCount = tourBookings.Count(b => b.CreatedDate >= todayStart && b.CreatedDate < nextDayStart);
                var previousDayCount = tourBookings.Count(b => b.CreatedDate >= previousDayStart && b.CreatedDate < todayStart);

                var totalTourBookings = bookingsPerTour is not null && bookingsPerTour.Any() && bookingsPerTour.TryGetValue(t.Id, out var count)
                                        ? count
                                        : 0;

                return new TopPerformingActivityDTO
                {
                    ActivityName = translation.Title,
                    ActivityImage = t.Images.FirstOrDefault()?.ImageUrl ?? "",
                    MarketDemand = DetermineMarketDemand(totalTourBookings, maxBookings),
                    Bookings = totalTourBookings,
                    Location = destTranslation.Name,
                    Trend = CalculateTrend(currentDayCount, previousDayCount)
                };
            }).ToList();

            var totalPages = query.PageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)query.PageSize);

            return Result.Ok(new PaginatedResponse<TopPerformingActivityDTO>
            {
                Data = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = totalPages,
                TotalItems = totalCount
            });
        }

        public async Task<Result<PaginatedResponse<AdminCustomerResponseDTO>>> GetCustomersAsync(GetAllCustomersQuery query)
        {



            var adminIds = await _userRepo.GetUserIdsInRoleAsync("Admin");

            var spec = new AdminCustomersFilterSpecification(query, adminIds);



            var users = await _userRepo.GetAllAsync(spec);
            var totalItems = await _userRepo.GetCountSpecificationAsync(new CountAdminCustomersFilterSpecification(query, adminIds));

            var vipIds = await _userRepo.GetUserIdsInRoleAsync("VIP");
            var booking = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetAllBookingSpecification());
            var bookingWithStudent = booking
                    .GroupBy(x => x.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            var payment = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(new GetAllSucceedPaymentSpecification());

            var paymentWithStudent = payment
                  .GroupBy(x => x.Order.UserId)
                  .ToDictionary(
                      g => g.Key,
                      g => g.Sum(x => x.TotalAmount)
                  );

            var items = users.Select(u =>
            {
                var country = string.IsNullOrWhiteSpace(u.Address?.Country)
                    ? "—"
                    : u.Address!.Country!;

                var since = u.CreatedDate.Year < 2001
                    ? DateTime.UtcNow
                    : u.CreatedDate;

                var isVip = vipIds.Contains(u.Id);

                return new AdminCustomerResponseDTO(
                    Id: u.Id,
                    CustomerCode: $"CUST-{u.Id[..Math.Min(4, u.Id.Length)].ToUpper()}",
                    FullName: u.FullName ?? "User",
                    AvatarUrl: u.ImageUrl,
                    Email: u.Email ?? "",
                    PhoneNumber: u.PhoneNumber,
                    Country: country,
                    Since: since.ToString("MMM dd, yyyy"),
                   Bookings: bookingWithStudent.TryGetValue(u.Id, out var bookingsCount)
                                        ? bookingsCount
                                        : 0,
                    Spend: paymentWithStudent.TryGetValue(u.Id, out var spend)
                            ? spend
                            : 0,
                    Status: u.IsActive ? "active" : "inactive",
                    IsVip: isVip,
                    UserType: isVip ? "VIP" : "Public"
                );
            }).ToList();
            var totalPages = query.PageSize <= 0
           ? 0
           : (int)Math.Ceiling(totalItems / (double)query.PageSize);

            var paginated = new PaginatedResponse<AdminCustomerResponseDTO>
            {
                Data = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,

            };


            return Result.Ok(paginated);
        }

        public async Task<Result<UpdateVipResponseDTO>> UpdateVipStatusAsync(string id, UpdateVipStatusRequestDTO request)
        {
            var user = await _userRepo.GetByIdAsync(new GetUserByIdSpecification(id));
            if (user is null)
            {
                return Result.Fail(new NotFoundError("User Not Found"));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return Result.Fail("Cannot change VIP for admin users.");

            var isVip = await _userManager.IsInRoleAsync(user, "VIP");

            // Promote to VIP
            if (request.IsVip && !isVip)
            {
                await _userManager.AddToRoleAsync(user, "VIP");
            }

            // Demote to Public (remove VIP role)
            if (!request.IsVip && isVip)
            {
                await _userManager.RemoveFromRoleAsync(user, "VIP");
            }
            return Result.Ok(new UpdateVipResponseDTO(Id: id, IsVIP: request.IsVip));
        }

        private async Task<string> ComputeMonthlyGrowth(List<string> adminIds)
        {

            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonthStart = currentMonthStart.AddMonths(1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);


            var currentMonthCustomers = await _userRepo.GetCountSpecificationAsync(new GetUserWithCurrentMonthSpecification(currentMonthStart, nextMonthStart, adminIds));


            var previousMonthCustomers = await _userRepo.GetCountSpecificationAsync(new GetUserWithPreviousMonthSpecification(currentMonthStart, previousMonthStart, adminIds));


            decimal growthPercent;
            if (previousMonthCustomers <= 0)
            {
                growthPercent = currentMonthCustomers > 0 ? 100m : 0m;
            }
            else
            {
                growthPercent = ((currentMonthCustomers - previousMonthCustomers) / (decimal)previousMonthCustomers) * 100m;
            }
            var growthText = $"{(growthPercent >= 0 ? "+" : "")}{Math.Round(growthPercent, 1):0.#}%";

            return growthText;
        }

        private List<DailyRevenueDTO> CalculateDailyRevenue(List<Payment> payments)
        {
            var dailyRevenue = new List<DailyRevenueDTO>();
            var daysInMonth = DateTime.UtcNow.Month switch
            {
                2 => DateTime.IsLeapYear(DateTime.UtcNow.Year) ? 29 : 28,
                4 or 6 or 9 or 11 => 30,
                _ => 31
            };

            for (int day = 1; day <= daysInMonth; day++)
            {
                var revenueForDay = payments
                    .Where(p => p.CreatedDate.Day == day)
                    .Sum(p => p.TotalAmount);

                dailyRevenue.Add(new DailyRevenueDTO
                {
                    Day = day,
                    Revenue = revenueForDay
                });
            }

            return dailyRevenue;
        }

        private List<RegionalMixDTO> CalculateRegionalMix(List<Booking> bookings)
        {
            var totalRevenue = bookings.Sum(b => b.OrderItem?.Order?.TotalAmount ?? 0);

            var regionalData = bookings
                .GroupBy(b => b.OrderItem?.DestinationName ?? "Unknown")
                .Select(g => new RegionalMixDTO
                {
                    Location = g.Key,
                    Percentage = (int)(totalRevenue > 0 ? (g.Sum(b => b.OrderItem?.Order?.TotalAmount ?? 0) * 100 / totalRevenue) : 0)
                })
                .OrderByDescending(r => r.Percentage)
                .Take(10)
                .ToList();

            return regionalData;
        }

        private List<TopPerformingActivityDTO> GetTopPerformingActivities(List<Booking> currentBookings, List<Booking> previousBookings)
        {
            var previousCounts = previousBookings
                .GroupBy(b => b.OrderItem?.TourTitle)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());

            var topActivities = currentBookings
                .GroupBy(b => new { b.OrderItem?.TourTitle, b.OrderItem?.DestinationName })
                .Select(g =>
                {
                    var tourTitle = g.Key?.TourTitle ?? "Unknown";
                    int currentCount = g.Count();
                    previousCounts.TryGetValue(tourTitle, out int previousCount);

                    return new TopPerformingActivityDTO
                    {
                        ActivityName = tourTitle,
                        ActivityImage = "",
                        MarketDemand = DetermineMarketDemand(currentCount, 100),
                        Bookings = currentCount,
                        Location = g.Key?.DestinationName ?? "Unknown",
                        Trend = CalculateTrend(currentCount, previousCount)
                    };
                })
                .OrderByDescending(a => a.Bookings)
                .Take(5)
                .ToList();

            return topActivities;
        }

        private string CalculateTrend(int current, int previous)
        {
            if (previous == 0) return current > 0 ? "↑100%" : "↑0.0%";

            decimal growth = ((current - previous) / (decimal)previous) * 100m;
            string arrow = growth >= 0 ? "↑" : "↓";
            return $"{arrow}{Math.Abs(Math.Round(growth, 1)):0.#}%";
        }

        private string DetermineMarketDemand(int bookingCount, int maxBookings)
        {

            if (maxBookings == 0) return "LOW";
            var percentage = bookingCount * 100m / maxBookings;

            if (percentage >= 90) return "VERY HIGH";
            if (percentage > 60) return "HIGH";
            if (percentage >= 30) return "MODERATE";

            return "LOW";
        }
      
       
       
       
    }
}
