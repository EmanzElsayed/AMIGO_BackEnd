using Amigo.Domain.Enum;
using System;

namespace Amigo.Domain.DTO.Booking
{
    public class UserBookingDTO
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public Guid TourId { get; set; }
        public string TourTitle { get; set; }
        public string DestinationName { get; set; }
        public DateOnly DateIso { get; set; }
        public string StartTime { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal PaidAmount { get; set; }
        public string Currency { get; set; }
        public string? ImageUrl { get; set; }
    }
}
