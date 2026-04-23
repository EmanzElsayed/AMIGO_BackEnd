using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Customer
{
    public record CreateAccountRequestDTO
    (
        
        string FirstName,
        string LastName,
        string Email,
        string CountryIsoCode,
        string PhoneNumber,
        string? ReturnUrl


    );
}
