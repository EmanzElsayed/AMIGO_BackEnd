using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IValidationService
    {
        Task<Result> ValidateAsync<T>(T model);
    }
}
