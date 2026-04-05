using Amigo.Domain.DTO.Destination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Destination
{
    public class UpdateActivationDestinationRequestDTOValidator:AbstractValidator<UpdateActivationDestinationRequestDTO>
    {
        public UpdateActivationDestinationRequestDTOValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Active Status Required");
        }
    }
}
