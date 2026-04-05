using Amigo.Application.Abstraction.Services;

using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.Enum;
using Amigo.Presentation.Filters;
using Amigo.SharedKernal.DTOs.Authentication;


namespace Amigo.Presentation.Controllers
{
    
    [Route("api/v1/lookups")]
    public class EnumsController(IEnumService _enumService):BaseController
    {
        [HttpGet("languages")]
        public  IResultBase GetLanguages()
        {
           return _enumService.GetEnum<Language>() ;

         

        }

        [HttpGet("Currency")]
        public IResultBase GetCurrency()
        {
           return _enumService.GetEnum<Currency>();

            

        }

        [HttpGet("Country")]
        public IResultBase GetCountry()
        {
            return _enumService.GetEnum<CountryCode>();



        }
    }
}
