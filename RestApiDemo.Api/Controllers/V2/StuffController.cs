using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RestApiDemo.Api.Controllers.V2
{
    /// <summary>
    /// Demo controller to demonstrate versioning features.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StuffController : ControllerBase
    {
        /// <summary>
        /// Get version 2.0 stuff. Check out the other version 1.0 as well!
        /// </summary>
        /// <returns>Version 2.0 stuff.</returns>
        [HttpGet(Name = "Stuff_GetV2")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<string> Get() => "This is V2.0 stuff";
    }
}