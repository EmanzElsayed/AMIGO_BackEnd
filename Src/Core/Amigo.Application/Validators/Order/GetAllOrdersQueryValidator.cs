using Amigo.Application.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Validators.Order
{
    public class GetAllOrdersQueryValidator:AbstractValidator<GetAllOrdersQuery>
    {
        public GetAllOrdersQueryValidator()
        {
            RuleFor(o => o.OrderStatus)
                .Must(BusinessRules.BeAValidOrderStatus)
                .When(o => !string.IsNullOrWhiteSpace(o.OrderStatus))
                .WithMessage("Invalid Order Status");

            RuleFor(o => o.PaymentStatus)
                .Must(BusinessRules.BeAValidPaymentStatus)
                .When(o => !string.IsNullOrWhiteSpace(o.PaymentStatus))
                .WithMessage("Invalid Payment Status");

            RuleFor(o => o.BookingStatus)
                .Must(BusinessRules.BeAValidBookingStatus)
                .When(o => !string.IsNullOrWhiteSpace(o.BookingStatus))
                .WithMessage("Invalid Booking Status");
        }
    }
}
