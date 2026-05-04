using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.User
{
    public  record AdminCustomerResponseDTO(
         string Id,
         string CustomerCode,
         string FullName,
         string? AvatarUrl,
         string Email,
         string? PhoneNumber,
         string Country,
         string Since,
         int Bookings,
         decimal Spend,
         string Status,
         bool IsVip,
         string UserType
    );
}
