using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminTourInclusionService
    {
        

        Task UpdateInclusionAsync(
            Tour tour,
            List<string>? includedList,
            List<string>? excludedList,
            Language? language);
    }
}
