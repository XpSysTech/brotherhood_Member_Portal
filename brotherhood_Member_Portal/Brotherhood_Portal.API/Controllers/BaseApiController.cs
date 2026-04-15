using Microsoft.AspNetCore.Mvc;

namespace Brotherhood_Portal.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
