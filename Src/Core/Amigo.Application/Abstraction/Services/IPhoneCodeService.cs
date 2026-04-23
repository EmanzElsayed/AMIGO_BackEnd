using Amigo.Domain.DTO.PhoneNumber;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPhoneCodeService
    {
        Result<IEnumerable<GetPhoneNumberCodeDTO>> GetCountries(string? lang);

    }
}
