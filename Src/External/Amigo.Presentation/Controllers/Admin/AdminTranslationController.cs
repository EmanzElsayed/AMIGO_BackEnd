using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Enums;
using Amigo.Domain.DTO.Tour;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/translate")]
    public class AdminTranslationController( IServiceManager _serviceManager ) 
        : BaseController

    {
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IResultBase> TranslateTours([FromBody] GetLanguageFromBodyDTO requestDTO)
        {

            return await _serviceManager.AutoTranslationService.TranslateAllPendingTours(requestDTO);

        }
    }
}
