using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validators.Images
{
    public class ImageUrlsRequestDTOValidator:AbstractValidator<ImageUrlsRequestDTO>
    {
        public ImageUrlsRequestDTOValidator()
        {
            RuleFor(x => x.PublicId)
           .NotEmpty()
           .When(x => !string.IsNullOrEmpty(x.ImageUrl))
           .WithMessage("PublicId is required when ImageUrl is provided");
        }
    }
}
