# Rate limiting

Request rate limiting can be added to the API using the **AspNetCoreRateLimit** NuGet package.

After installing the NuGet package, add the following to ConfigureServices() in Startup.cs. These settings will use in-memory caching of request counts to calculate the rate limits for clients.

```csharp
services.AddOptions();
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
```

At the beginning of Configure() in Startup.cs, before any other call add the following. Rate limiting should be the first part of the handling requests, and since `app.Use..()` go in order, the rate limiting call should be first.

```csharp
app.UseIpRateLimiting();
```

The rate limiting is configured in the appsettings.json. The following section can be added. See the [GitHub documentation](https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup) for the Middleware for more details on configuration.

```json
"IpRateLimiting": {
  "EnableEndpointRateLimiting": true,
  "StackBlockedRequests": false,
  "RealIpHeader": "X-Real-IP",
  "ClientIdHeader": "X-ClientId",
  "HttpStatusCode": 429,
  "GeneralRules": [
    {
      "Endpoint": "*",
      "Period": "1s",
      "Limit": 2
    },
    {
      "Endpoint": "*",
      "Period": "15m",
      "Limit": 100
    },
    {
      "Endpoint": "*",
      "Period": "12h",
      "Limit": 1000
    },
    {
      "Endpoint": "*",
      "Period": "7d",
      "Limit": 10000
    },
    {
      "Endpoint": "*:/api/v*/stuff",
      "Period": "1m",
      "Limit": 5
    },
    {
      "Endpoint": "POST:/api/v*/meals",
      "Period": "1m",
      "Limit": 5
    }
  ]
}
```

The last two rules show examples of setting rate limit rules only for certain endpoints. The last one limits only POST requests to the meals APIs (any version) and the rule before that limits all requests to the stuff APIs.

There are additional settings that can be added to the IpRateLimiting section in the appsettings.json. Examples of settings are the following.

| Type | Example |
| ---- | ------- |
| IP addresses to exclude from rate limiting | `"IpWhitelist": [ "127.0.0.1", "::1/10", "192.168.0.0/24" ]` |
| API endpoints to exclude from rate limiting | `"EndpointWhitelist": [ "get:/api/license", "*:/api/status" ]` |
| Clients to exclude from rate limting | `"ClientWhitelist": [ "dev-id-1", "dev-id-2" ]` |

## Rate limit responses

The middleware will add headers to the response that detail the rate limit for the requested endpoint. Example headers for a get request, in this case there are 9977 requests remaining in the next 7 days before requests will be rejected:

```http
HTTP/1.1 200 OK
X-Rate-Limit-Limit: 7d
X-Rate-Limit-Remaining: 9977
X-Rate-Limit-Reset: 2019-05-04T06:00:22.6148159Z
```

If the rate limit exceed a 429 Too Many Requests response is returned, together with a `Retry-After` header. Example response, in this case the rate limit was a maximum of 2 calls per 1 second:

```http
HTTP/1.1 429 Too Many Requests
Transfer-Encoding: chunked
Content-Type: text/plain
Retry-After: 1
Server: Microsoft-IIS/10.0
X-SourceFiles: =?UTF-8?B?RDpcUmFwaGlcUHJvZ3JhbW1pbmdcY3NoYXJwXFJlc3RBcGlEZW1vLk5ldENvcmVcUmVzdEFwaURlbW8uQXBpXGFwaVx2MVxpbmdyZWRpZW50cw==?=
X-Powered-By: ASP.NET
Date: Sat, 27 Apr 2019 06:00:34 GMT
Connection: close

API calls quota exceeded! maximum admitted 2 per 1s.
```