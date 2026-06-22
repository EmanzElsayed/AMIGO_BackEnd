using Amigo.Application.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/contact-info")]

    public class ContactInfoController(IServiceManager _serviceManager) : BaseController
    {
        [HttpGet]
        public IResultBase GetContactInfo()
        {
            return _serviceManager.EnumService.GetContactInfo();


        }
    }
}
