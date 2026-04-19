using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public class AdminTourListItemResponseDTO
    {
        public Guid TourId { get; set; }
        public string? Title { get; set; }
        public string? DestinationName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? EntryAmountVIP { get; set; }
        public string? EntryAmountVIPLabel { get; set; }
        public decimal? EntryAmountPublic { get; set; }
        public string? EntryAmountPublicLabel { get; set; }
        public int TotalCapacity { get; set; }
        public int BookedSeats { get; set; }
        public int BookedPercentage { get; set; }
        public string Status { get; set; } = "Active";
    }

}
