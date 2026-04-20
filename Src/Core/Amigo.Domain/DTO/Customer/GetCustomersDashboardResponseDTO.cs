using Amigo.SharedKernal.DTOs.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Customer
{
    public class GetCustomersDashboardResponseDTO
    {
        public int VipSegments { get; set; }

        public string GrowthText { get; set; }

        public int GlobalTravelers { get; set; }

    }
}
