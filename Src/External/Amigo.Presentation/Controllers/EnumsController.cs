using Amigo.Application.Abstraction.Services;
using Amigo.Application.Services;
using Amigo.Domain.DTO.Authentication;
using Amigo.Domain.DTO.Currency;
using Amigo.Domain.Enum;
using Amigo.Presentation.Filters;
using Amigo.SharedKernal.DTOs.Authentication;
using Amigo.SharedKernal.QueryParams;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace Amigo.Presentation.Controllers
{
    
    [Route("api/v1/lookups")]
    public class EnumsController(
        
        IServiceManager _serviceManager
       ) : BaseController
    {
        

        [HttpGet("languages")]
        public IResultBase GetLanguages()
        {
            return _serviceManager.EnumService.GetLanguageEnum();

         
        }

      

        [HttpGet("Currency")]
        public async Task<IResultBase> GetAllCurrency([FromQuery]  GetAllCurrencyQuery requestQuery)
        {
            return await _serviceManager.CurrencyService.GetAllCurrencyAsync(requestQuery);

        }
        [HttpGet("Currency/{id}")]
        public async Task<IResultBase> GetCurrencyById([FromQuery] GetLanuageQuery requestQuery, string id)
        {
            return await _serviceManager.CurrencyService.GetCurrencyByIdAsync(id,requestQuery);

        }


        [HttpGet("Country-info")]
        public async Task<IResultBase> GetAllCountryInfo([FromQuery] GetAllCountryInfoQuery requestQuery)
        {
            return await _serviceManager.CountryInfoService.GetAllCountryInfoAsync(requestQuery);

        }
        [HttpGet("Country-info/{id}")]
        public async Task<IResultBase> GetCountryInfoById([FromQuery] GetLanuageQuery requestQuery, string id)
        {
            return await _serviceManager.CountryInfoService.GetCountryInfoByIdAsync(id, requestQuery);

        }


       
    }
}
