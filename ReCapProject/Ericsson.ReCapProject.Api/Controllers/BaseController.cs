using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ericsson.ReCapProject.Api.Controllers
{
    [Authorize(Policy = "AzureReCapProjectPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
    }
}
