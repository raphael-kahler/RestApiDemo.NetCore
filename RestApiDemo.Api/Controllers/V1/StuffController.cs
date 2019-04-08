using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RestApiDemo.Api.Controllers.V1
{
    /// <summary>
    /// Demo controller to demonstrate versioning features.
    /// </summary>
    [ApiVersion("1.0", Deprecated = true)] // Versions can be marked as deprecated. For this to work you need this setup: services.AddApiVersioning( options => options.ReportApiVersions = true );
    [AdvertiseApiVersions("3.0")] // Advertise in the return headers that a v3 of the API exists (which is a lie in this case). This would be used if v3 was implemented in a different project that still maps to the same url. For this to work you need this setup: services.AddApiVersioning( options => options.ReportApiVersions = true );
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StuffController : ControllerBase
    {
        /// <summary>
        /// Get version 1.0 stuff. Check out the other version 2.0 as well!
        /// </summary>
        /// <returns>Version 1.0 stuff.</returns>
        [HttpGet(Name = "Stuff_GetV1")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<string> Get() => "This is V1.0 stuff";
    }
}