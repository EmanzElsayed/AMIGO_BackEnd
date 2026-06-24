using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Search;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1")]

    public class ContactInfoController(IServiceManager _serviceManager) : BaseController
    {
        [HttpGet("contact-info")]
        public IResultBase GetContactInfo()
        {
            return _serviceManager.EnumService.GetContactInfo();


        }
        [HttpGet("search")]
        public async Task<IResultBase> searchQueyr([FromQuery]SearchQueryRequestDTO requestDTO)
        {
            return await _serviceManager.DestinationService.SearchQuery(requestDTO);


        }
    }
}
