using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class CountGetAllCancellationRequestsSpecification : BaseSpecification<CancellationRequest, Guid>
    {
        public CountGetAllCancellationRequestsSpecification(GetAllAdminCancellationRequestQuery requestQuery)
            : base(c => !c.IsDeleted &&
                (string.IsNullOrWhiteSpace(requestQuery.UserName) || c.Booking.CustomerName.Contains(requestQuery.UserName.Trim().ToLower()))
                &&
                (string.IsNullOrWhiteSpace(requestQuery.UserEmail) || c.Booking.CustomerEmail == requestQuery.UserEmail)
                && (string.IsNullOrWhiteSpace(requestQuery.BookingNumber) || c.Booking.BookingNumber == requestQuery.BookingNumber)
                && (string.IsNullOrWhiteSpace(requestQuery.CancellationStatus) || c.Status == EnumsMapping.ToEnum<CancellationRequestStatus>(requestQuery.CancellationStatus, false))

            )
        { 
            
        }
    }
}
