using Amigo.Application.Abstraction;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application;

    public class UserMapping : IUserMapping
    {
        public ApplicationUser ToEntity(RegisterRequestDTO requestDTO)
        => new ApplicationUser()
        {
            Email = requestDTO.Email,
            UserName = requestDTO.Email.Split('@')[0]
        };
            
    }

