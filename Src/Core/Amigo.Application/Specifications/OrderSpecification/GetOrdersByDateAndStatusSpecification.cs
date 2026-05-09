using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetOrdersByDateAndStatusSpecification : BaseSpecification<Order, Guid>
    {
        public GetOrdersByDateAndStatusSpecification(DateTime startDate, DateTime endDate, OrderStatus status)
            : base(o => !o.IsDeleted && o.Status == status && o.OrderDate >= startDate && o.OrderDate < endDate)
        {
        }
    }
}
