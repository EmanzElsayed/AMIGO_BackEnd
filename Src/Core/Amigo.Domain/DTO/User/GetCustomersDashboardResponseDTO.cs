using Amigo.SharedKernal.DTOs.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.User
{
    public class GetCustomersDashboardResponseDTO
    {
        public int BookingsThisMonth { get; set; }

        public decimal GrossRevenue { get; set; }

        public int TotalCustomers { get; set; }

        public int VipMembers { get; set; }

        public string MonthlyGrowthText { get; set; }

        public List<DailyRevenueDTO> DailyRevenue { get; set; } = new();

        public List<RegionalMixDTO> RegionalMix { get; set; } = new();

        public List<TopPerformingActivityDTO> TopPerformingActivities { get; set; } = new();
    }

    public class DailyRevenueDTO
    {
        public int Day { get; set; }

        public decimal Revenue { get; set; }
    }

    public class RegionalMixDTO
    {
        public string Location { get; set; }

        public int Percentage { get; set; }
    }

    public class TopPerformingActivityDTO
    {
        public string ActivityName { get; set; }

        public string ActivityImage { get; set; }

        public string MarketDemand { get; set; }

        public int Bookings { get; set; }

        public string Location { get; set; }

        public string Trend { get; set; }
    }
}
