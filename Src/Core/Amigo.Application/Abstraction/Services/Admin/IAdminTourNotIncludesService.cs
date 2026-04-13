using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminTourNotIncludesService
    {
        Task UpdateExcludesAsync(
          Tour tour,
          List<string>? excludes,
          Language? language);
    }
}
