

using Amigo.Domain.DTO.Enums;

namespace Amigo.Application.Abstraction.Services
{
    public interface IEnumService
    {
        Result<IEnumerable< GetEnumResponseDTO>> GetEnum<T>() where T : Enum;
        Result<List<GetLanguageResponseDTO>> GetLanguageEnum();

    }
}
