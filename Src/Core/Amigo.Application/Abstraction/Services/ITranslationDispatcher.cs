using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ITranslationDispatcher
    {
        Task HandleTourCreated(Guid tourId);
        Task HandleTourUpdated(Guid tourId);
    }
}
