using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/tour")]
    [Authorize(Roles = "Admin")]
    public class AdminTourController(IAdminTourService _adminTourService) 
        :BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateTour([FromBody] CreateTourRequestDTO requestDTO)
        {
           
            return await _adminTourService.CreateTourAsync(requestDTO);

        }
        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateTour([FromBody] UpdateTourRequestDTO requestDTO, string id)
        {

            return await _adminTourService.UpdateTourAsync(requestDTO, id);

        }
        [HttpGet]
        public async Task<IResultBase> GetTours([FromQuery] GetAllAdminTourQuery requestDTO)
        {

            return await _adminTourService.GetAllToursAsync(requestDTO);

        }
        [HttpGet("{id}")]
        public async Task<IResultBase> GetTourById(string id, [FromQuery] GetTourByIdRequestDTO requestDTO)
        {

            return await _adminTourService.GetTourById(id , requestDTO);

        }


        //    [HttpGet("stats")]
        //    public async Task<IResultBase> GetActivityStats()
        //    {
        //        var now = DateTime.UtcNow;
        //        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        //        var nextMonthStart = monthStart.AddMonths(1);

        //        var bookingsThisMonth = await db.Bookings.AsNoTracking()
        //            .Where(b => !b.IsDeleted && b.ConfirmedAt.HasValue && b.ConfirmedAt.Value >= monthStart && b.ConfirmedAt.Value < nextMonthStart)
        //            .CountAsync();

        //        var grossRevenue = await db.Orders.AsNoTracking()
        //            .Where(o => !o.IsDeleted && o.Status == OrderStatus.Paid && o.OrderDate.HasValue && o.OrderDate.Value >= monthStart && o.OrderDate.Value < nextMonthStart)
        //            .Select(o => (decimal?)o.TotalAmount)
        //            .SumAsync() ?? 0m;

        //        var totalCapacity = await db.AvailableSlots.AsNoTracking()
        //            .Where(s => !s.IsDeleted)
        //            .Select(s => (int?)s.MaxCapacity)
        //            .SumAsync() ?? 0;

        //        var totalBookedSeats = await (
        //            from oi in db.OrderItems.AsNoTracking()
        //            join o in db.Orders.AsNoTracking() on oi.OrderId equals o.Id
        //            where !oi.IsDeleted && !o.IsDeleted && o.Status == OrderStatus.Paid
        //            select (int?)oi.Quantity
        //        ).SumAsync() ?? 0;

        //        var avgCapacity = totalCapacity <= 0
        //            ? 0
        //            : Math.Clamp((int)Math.Round((decimal)totalBookedSeats * 100m / totalCapacity, MidpointRounding.AwayFromZero), 0, 100);

        //        var status = avgCapacity >= 90 ? "Low Stock" : "Active";

        //        return Result.Ok(new
        //        {
        //            bookingsThisMonth,
        //            avgCapacity,
        //            grossRevenue,
        //            status
        //        });
        //    }

        //    private static string EscapeLikePattern(string value)
        //    {
        //        return value
        //            .Replace("\\", "\\\\")
        //            .Replace("%", "\\%")
        //            .Replace("_", "\\_");
        //    }
        //}

    }
}
