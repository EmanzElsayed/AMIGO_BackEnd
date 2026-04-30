using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetUserBookingHistorySpecification : BaseSpecification<OrderItem, Guid>
    {
        public GetUserBookingHistorySpecification(string userId, string? paymentStatus = null)
            : base(item => item.Order.UserId == userId && !item.Order.IsDeleted && !item.IsDeleted
                && (string.IsNullOrWhiteSpace(paymentStatus) || !paymentStatus.Equals("Succeeded", StringComparison.OrdinalIgnoreCase)
                    ? true
                    : item.Order.Status == OrderStatus.Paid && item.Order.Payments.Any(p => p.Status == PaymentStatus.Succeeded && !p.IsDeleted)))
        {
            AddInclude(x => x
                .Include(i => i.Order)
                    .ThenInclude(o => o.Payments)
            );
        }
    }
}
