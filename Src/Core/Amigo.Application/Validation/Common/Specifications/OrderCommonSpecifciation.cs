using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Validation.Common.Specifications
{
    public static class OrderCommonSpecifciation
    {
        public static Expression<Func<Order, bool>> BuildCriteriaForUser
       (GetAllOrdersQuery query, string userId)
        {

            var tourName = string.IsNullOrWhiteSpace(query.TourTitle) ? "": query.TourTitle.Trim().ToLower() ;
            
            OrderStatus? status = string.IsNullOrWhiteSpace(query.OrderStatus)
                ? null
                : EnumsMapping.ToEnum<OrderStatus>(query.OrderStatus, false);

            return o => o.UserId == userId &&

                  (string.IsNullOrWhiteSpace(query.OrderStatus) || o.Status == status)
                  &&
                  (query.OrderDate != null || o.OrderDate == query.OrderDate)

                  && (string.IsNullOrWhiteSpace(query.TourTitle) ||

                  o.OrderItems.Any(i =>
                      !string.IsNullOrWhiteSpace(i.TourTitle) &&
                      i.TourTitle.Trim().ToLower().Contains(tourName)
                  ));
        }
    }
}
