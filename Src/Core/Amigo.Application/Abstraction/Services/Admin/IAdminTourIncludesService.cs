using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminTourIncludesService
    {
        Task UpdateIncludesAsync(
                    Tour tour,
                    List<string>? includes,
                    Language? language);
    }
}
