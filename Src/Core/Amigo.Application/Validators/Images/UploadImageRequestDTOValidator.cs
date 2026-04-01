using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Images
{
    public class UploadImageRequestDTOValidator :AbstractValidator<UploadImageRequestDTO>
    {
        public UploadImageRequestDTOValidator()
        {
            RuleFor(x => x.Image)
                .NotEmpty()
                .WithMessage("Image Required");
        }
    }
}
