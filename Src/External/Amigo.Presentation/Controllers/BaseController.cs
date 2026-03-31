
namespace Amigo.Presentation.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ResultFilter))]
    [Route("api/v1")]
    public class BaseController:ControllerBase
    {
    }
}
