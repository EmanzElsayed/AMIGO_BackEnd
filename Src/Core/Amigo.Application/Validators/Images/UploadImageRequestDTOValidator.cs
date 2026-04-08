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
                .NotNull()
                .WithMessage("Image Required");

            RuleFor(x => x.Image)
                      .Must(file => file.Length <= 10 * 1024 * 1024)
                      .WithMessage("Each image must be less than 10MB");
        }
    }
}
