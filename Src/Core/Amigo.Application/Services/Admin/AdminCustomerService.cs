using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.UserSpecification;
using Amigo.Application.Specifications.PaymentSpecification;
using Amigo.Domain.DTO.User;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var vipIds = await _userRepo.GetUserIdsInRoleAsync("VIP");
            var booking = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetAllBookingSpecification());

            int vipNumber = vipIds.Count();
            int globalTravelars = booking.Sum(b => b.Travelers.Count());
            var adminIds = await _userRepo.GetUserIdsInRoleAsync("Admin");

            string growthText = await ComputeMonthlyGrowth(adminIds);

            return Result.Ok(new GetCustomersDashboardResponseDTO
            {
                GlobalTravelers = globalTravelars,
                VipSegments = vipNumber,
                GrowthText = growthText,
            }
           );
        }

        public async Task<Result<PaginatedResponse<AdminCustomerResponseDTO>>> GetCustomersAsync(GetAllCustomersQuery query)
        {



            var adminIds = await _userRepo.GetUserIdsInRoleAsync("Admin");

            var spec = new AdminCustomersFilterSpecification(query,adminIds);



            var users = await _userRepo.GetAllAsync(spec);
            var totalItems = await _userRepo.GetCountSpecificationAsync(new CountAdminCustomersFilterSpecification(query,adminIds));

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
            return Result.Ok(new UpdateVipResponseDTO ( Id:id , IsVIP : request.IsVip ));
        }

        private async Task<string> ComputeMonthlyGrowth(List<string>adminIds)
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


    }
}
