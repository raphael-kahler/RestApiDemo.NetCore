# Caching

## Caching implementation using ResponseCaching middleware

This middleware comes as part of ASP.NET core. It takes care of almost all the cache implementations.
All that is required is to configure the middleware in the service startup and decorate the GET APIs that should be cached with `ResponseCache` attributes.
Details can be found in the [ASP.NET core caching middleware docs](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-2.2).

By default this middleware will use in-memory caching.

In Startup.cs add the following in ConfigureServices():

```csharp
services.AddResponseCaching(options =>
{
    options.UseCaseSensitivePaths = false; // whether different cache responses should be stored based on case sensitive paths, default is false
    options.MaximumBodySize = 64 * 1024 * 1024; // in bytes; largest cachable size for a single response body, default is 64 MB
    options.SizeLimit = 500 * 1024 * 1024; // in bytes; max size of the middleware cache, default is 100 MB
});
```

Also add the following in Startup.cs in Configure():

```csharp
// note: the app.Use...() methods go in order and can short circuit later app.Use...() methods. Make sure UseResponseCaching() comes before UseMVC(), otherwise MVC will serve the results before the cache gets hit.
app.UseResponseCaching();
//other stuff
app.UseMVC();
```

In your controllers, decorate either the controller or the methods with `ResponseCache` attributes. See the [documentation](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-2.2#responsecache-attribute) for attribute values.

```csharp
// cache for a specified time
[HttpGet("{name}"]
[ResponseCache(Duration = 60)] // easiest setup, just specify the number of seconds the result should be cached on the server
public ActionResult<Ingredient> Get(string name)

// cache by query parameters
[HttpGet]
[ResponseCache(VaryByQueryKeys = new string[] { "count", "offset" }, Duration = 60)] // cache separate responses for each combination of count and offset for 60 seconds
public ActionResult<PagedResult<Meal>> Get(int count = 10, int offset = 0)

// disable caching
[HttpGet("{id}"]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true )] // to disable caching for a method set Location to none and NoStore to true
public ActionResult<Meal> Get(int id)
```

Now caching is enabled. The following table shows examples of caching scenarios and some http headers that can be used to modify the cache behavior.

| Scenario | Request | Result | Response headers |
| ----- | ------- | ------ | ---------------- |
| 1     | `GET ~/api/v1/ingredients/water` | The first GET will fetch data from the server and add it to the cache. Cache-Control header is returned that contains the cache entry details. 'max-age' is the duration that what was configured with the `ResponseCache` attribute (in the example above we set the duration to 60 seconds). | Cache-Control: public,max-age=60 |
| 2     | `GET ~/api/v1/ingredients/water` | The second GET returns data from the cache if the age has not expired yet. The response headers contain the age of the cache entry (in this case the entry is 11 seconds old). | Cache-Control: public,max-age=60<br/>Age: 11 |
| 3     | `GET ~/api/v1/ingredients/water`<br/>Cache-Control: no-cache | The client can add a **no-cache** header to the GET request to force the server to ignore the cache. The server then serves the result and updates the cache with it, resetting the cache age. | Cache-Control: public,max-age=60 |
| 4     | `GET ~/api/v1/ingredients/water`<br/>Cache-Control: no-store | The client can add a **no-store** header to the GET request to prevent the server from storing the response in the cache. If the response is already cached, the cached result is returned. | Cache-Control: public,max-age=60<br/>Age: 37 (if response was already cached) |
| 5     | `GET ~/api/v1/ingredients/water`<br/>Cache-Control: no-cache,no-store | By adding both **no-cache** and **no-store** to the GET request the client tells the server to not use any cached response and also to not add the served response to the cache. Cache-Control header is still returned by the server, but no new response is cached. | Cache-Control: public,max-age=60 |
| 6     | `GET ~/api/v1/ingredients/water`<br/>Cache-Control: max-age=10 | The client can add the **max-age** header to the GET request to only allow cached responses with a lower age. If the server cache is older, the server has to recreate the response. The cache is then refreshed with the new response | Cache-Control: public,max-age=60<br/>Age: 9 (if response is cached and age is within requested max-age) |