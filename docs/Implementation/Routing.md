# API routing with versioning

## Adding versioning to the project

Install the [API versioning NuGets](https://github.com/Microsoft/aspnet-api-versioning). For .NET core the NuGets are:

- `Microsoft.AspNetCore.Mvc.Versioning`
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer`

Then add the following to the ConfigureServices() method in Startup.cs.

```csharp
// Add API versioning services.
services.AddApiVersioning(options => options.ReportApiVersions = true /* true adds a header that shows the supported and deprecated versions of the API */);
services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true; // make help page show ~/api/v1/{controller}/ instead of ~/api/{version}/{controller}
    });
```

## Setting up versioned API routes

The next step is to decorate the API controllers with versioning attributes.
The route attribute on the controller can make use the the API version number.
The resulting route will be the base route for all API methods in this controller.

```csharp
[ApiVersion("1.0")] // the version of the API
[Route("api/v{version:apiVersion}/[controller]")] // add the version to the api route, in this case the route will look like: api/v1/ingredients
[ApiController]
public class IngredientsController : ControllerBase
```

### API verbs

Each controller method has to be set up with the correct route and supported HTTP verbs.
Typically method name is Get(), Post(), Put(), etc., which mimics the operation that this method should handle.
In addition an HttpMethod attribute is added to the method that signals the supported HTTP verb and the route for this method.
Examples for the attribute are `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, etc.

The typical usage of the attributes are `[HttpPost("relative\route\with\{id}", Name = "friendlyName")]`.

- The relative route is appended to the base controller route. Curly braces are used to extract that part of the route into a variable (see examples below).
- The friendly name supplied in the Name property is used when constructing and referencing routes programatically. It's best to give all routes a friendly name.

### Routing examples

```csharp
// controller base api is /api/v1/ingredients
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class IngredientsController : ControllerBase
{
    [HttpGet(Name = "Meals_GetAll")] // HttpGet does not specify a relative route, so the final route is GET /api/v1/ingredients
    [HttpHead(Name = "Meals_HeadAll")] // HttpHead is the same as get, and points to HEAD /api/v1/ingredients
    public ActionResult<PagedResult<Meal>> Get(int count = 10, int offset = 0)
    { /*do stuff*/ }

    // HttpGet specifies relative route, which makes the total route GET /api/v1/ingredients/{name}.
    // If a get request has route /api/v1/ingredients/flour, then {name} will match "flour", and "flour" gets passed to the name parameter of the method.
    [HttpHead("{name}", Name = "Ingredients_HeadsSingle")]
    [HttpGet("{name}", Name = "Ingredients_GetSingle")]
    public ActionResult<Ingredient> Get(string name)
    { /*do stuff*/ }

    // HttpPost specifies no route, so the final route is POST /api/v1/ingredients
    // The method uses [FromBody] on its parameter. This will then parse the request body of a POST request
    // and attempt to convert the content into an instance of the class IngredientCreation.
    // Only one [FromBody] is allowed per method.
    [HttpPost(Name = "Ingredients_PostNew")]
    public ActionResult<Ingredient> Post([FromBody] IngredientCreation newIngredient)
    { /*do stuff*/ }
}
```

## Multiple versions of an API

If you have both a V1 and and V2 version of an API it is best to separate the versions into different controllers.
One strategy is to use namespaces and have the same controller name and method name in a different namespace which contains the version number.

```csharp
namespace RestApiDemo.Api.Controllers.V1 // namespace contains the version number
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StuffController : ControllerBase
    {
        [HttpGet(Name = "Stuff_GetV1")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<string> Get() => "This is V1.0 stuff";
    }
}

namespace RestApiDemo.Api.Controllers.V2 // version 2 of the API lives in a different namespace
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StuffController : ControllerBase
    {
        [HttpGet(Name = "Stuff_GetV2")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<string> Get() => "This is V2.0 stuff";
    }
}
```

If you initialized the API versioning at startup with `services.AddApiVersioning(options => options.ReportApiVersions = true)`, then each API response will include headers that announce the supported and deprecated versions of the called API.
It is possible to mark APIs as deprecated in the `ApiVersion` attribute.

```csharp
[ApiVersion("1.0", Deprecated = true)]
```

In a case where version 1 is deprecated, and version 2 is not deprecated, the response headers would include this:

```http
HTTP/1.1 200 OK
api-supported-versions: 2.0
api-deprecated-versions: 1.0
```