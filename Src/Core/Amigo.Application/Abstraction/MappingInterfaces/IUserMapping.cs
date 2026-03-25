using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction;

public interface IUserMapping
{
    ApplicationUser ToEntity(RegisterRequestDTO requestDTO);
}
