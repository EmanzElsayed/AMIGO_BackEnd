

namespace Amigo.Application;

    public class UserMapping : IUserMapping
    {
        public ApplicationUser ToEntity(RegisterRequestDTO requestDTO)
        => new ApplicationUser()
        {
            Email = requestDTO.Email,
            UserName = requestDTO.Email.Split('@')[0],
            FullName = requestDTO.FullName,
        };
            
    }

