using Amigo.Application.Services.Translation;
using Amigo.Domain.DTO.Destination;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/translation")]
    //[Authorize(Roles = "Admin")]
    public class AdminTranslationController:BaseController
    {
        private readonly TranslationEngine _engine;

        public AdminTranslationController(TranslationEngine engine)
        {
            _engine = engine;
        }
        [HttpPost("destination")]
        public async Task<IActionResult> TranslateDestination([FromBody] DestinationTranslateRequestDTO request)
        {
            await _engine.TranslateDestination(request.DestinationId, request.SourceLanguage);

            return Ok("Translated successfully");
        }
    }
}
