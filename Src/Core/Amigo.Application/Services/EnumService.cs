

namespace Amigo.Application.Services
{
    public class EnumService : IEnumService
    {
        public Result<IEnumerable< GetEnumResponseDTO>> GetEnum<T>() where T : Enum
        {
            var result =  Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(e => new GetEnumResponseDTO(

                        Id: Convert.ToInt32(e)
                        , Name: e.ToString()


                    )

            );
            
            return Result.Ok( result)
                         .WithSuccess(new Success("Get Enum Successfully!!"));



        }
    }
}
