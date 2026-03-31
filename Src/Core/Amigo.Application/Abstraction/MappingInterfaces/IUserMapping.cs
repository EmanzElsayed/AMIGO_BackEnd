

namespace Amigo.Application.Abstraction;

public interface IUserMapping
{
    ApplicationUser ToEntity(RegisterRequestDTO requestDTO);
}
