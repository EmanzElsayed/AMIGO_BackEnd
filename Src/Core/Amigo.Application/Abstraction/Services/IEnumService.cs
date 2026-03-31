

namespace Amigo.Application.Abstraction.Services
{
    public interface IEnumService
    {
        Result<IEnumerable< GetEnumResponseDTO>> GetEnum<T>() where T : Enum;
    }
}
