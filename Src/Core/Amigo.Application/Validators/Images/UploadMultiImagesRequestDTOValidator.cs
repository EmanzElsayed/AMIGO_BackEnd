using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Images
{
    public class UploadMultiImagesRequestDTOValidator :AbstractValidator<UploadMultiImagesRequestDTO>
    {
        public UploadMultiImagesRequestDTOValidator()
        {
            RuleFor(x => x.Images)
                    .NotNull()
                    .WithMessage("Images are required")
                    .Must(x => x.Count > 0)
                    .WithMessage("At least one image is required");

            RuleForEach(x => x.Images)
                    .NotNull()
                    .WithMessage("Invalid image file");

            RuleForEach(x => x.Images)
                        .Must(file => file.Length <= 10 * 1024 * 1024)
                        .WithMessage("Each image must be less than 10MB");
        }
    }
}
