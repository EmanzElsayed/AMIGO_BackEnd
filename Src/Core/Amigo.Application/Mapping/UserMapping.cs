

using Amigo.Domain.DTO.User;

namespace Amigo.Application;

    public static class UserMapping 
    {
        public static ApplicationUser ToEntity(this RegisterRequestDTO requestDTO)
        => new ApplicationUser()
        {
            Email = requestDTO.Email,
            UserName = requestDTO.Email.Split('@')[0],
            FullName = requestDTO.FullName,
        };

    public static UserInfoResponseDTO ToUserDTO(this ApplicationUser user,string role)
    {
        return new UserInfoResponseDTO(
                FullName:user.FullName,
                Email:user.Email,
                Phone: user.PhoneNumber,
                ImageUrl : user.ImageUrl,
                Gender : user.Gender.ToString(),
                BirthDate : user.BirthDate,
                Nationality : user.Nationality,
                Language : user.Language.ToString(),
                BuildingNumber:user.Address is null? "": user.Address.BuildingNumber,
                City : user.Address is null?""  : user.Address.City,
                Country : user.Address is null ? "": user.Address.Country,
                UserType: role
                    
        );
    }
}

