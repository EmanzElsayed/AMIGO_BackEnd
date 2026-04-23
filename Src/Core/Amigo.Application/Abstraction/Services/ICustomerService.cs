using Amigo.Domain.DTO.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ICustomerService
    {
        Task<Result<LoginResponseDTO>> ContinueWithEmail(CreateAccountRequestDTO requestDTO);   
            
     }
}
