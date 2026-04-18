//using Amigo.Application.Abstraction.Services;
//using Amigo.Domain.Entities;
//using Amigo.Domain.Enum;
//using Microsoft.EntityFrameworkCore;

//namespace Amigo.Persistence.Services;

//public class TourReviewEligibilityReader(AmigoDbContext db) : ITourReviewEligibilityReader
//{
//    public async Task<bool> CanUserWriteReviewAsync(
//        string userId,
//        Guid tourId,
//        CancellationToken cancellationToken = default)
//    {
//        var alreadyReviewed = await db.Reviews.AsNoTracking()
//            .AnyAsync(
//                r => !r.IsDeleted && r.TourId == tourId && r.UserId == userId,
//                cancellationToken);
//        if (alreadyReviewed)
//            return false;

//        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);

//        var eligible = await (
//            from b in db.Bookings.AsNoTracking()
//            join o in db.Orders.AsNoTracking() on b.OrderId equals o.Id
//            join pay in db.Payments.AsNoTracking() on o.Id equals pay.OrderId
//            join s in db.Set<AvailableSlots>().AsNoTracking() on b.AvailableSlotsId equals s.Id
//            join ts in db.TourSchedules.AsNoTracking() on s.TourScheduleId equals ts.Id
//            where !b.IsDeleted && !o.IsDeleted && !pay.IsDeleted && !s.IsDeleted && !ts.IsDeleted
//                  && o.UserId == userId
//                  && ts.TourId == tourId
//                  && pay.Status == PaymentStatus.Completed
//                  && o.Status == OrderStatus.Paid
//                  && (b.Status == BookingStatus.Completed || ScheduleServiceWindowEnded(ts, todayUtc))
//            select b.Id).AnyAsync(cancellationToken);

//        return eligible;
//    }

//    private static bool ScheduleServiceWindowEnded(TourSchedule ts, DateOnly todayUtc)
//    {
//        var lastDay = ts.EndDate ?? ts.StartDate;
//        return lastDay < todayUtc;
//    }
//}
