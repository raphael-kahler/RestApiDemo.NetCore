# REST API cheatsheet

Table of Contents:

- [REST API cheatsheet](#rest-api-cheatsheet)
  - [RESTful routes](#restful-routes)
  - [HTTP request methods](#http-request-methods)
  - [Safe and idempotent actions](#safe-and-idempotent-actions)
  - [Responses for HTTP requests](#responses-for-http-requests)
  - [API Versioning](#api-versioning)
  - [OpenAPI documentation for API discovery](#openapi-documentation-for-api-discovery)
  - [Caching](#caching)
    - [ETags](#etags)
    - [Cache-Control](#cache-control)
      - [Response headers](#response-headers)
      - [Request headers](#request-headers)
    - [Cache Types](#cache-types)
  - [Rate limiting](#rate-limiting)
  - [Use Data Transfer Objects (DTOs)](#use-data-transfer-objects-dtos)
    - [Domain models](#domain-models)
    - [Data persistence models](#data-persistence-models)
    - [DTOs](#dtos)

## RESTful routes

[Implementation guidelines](/docs/Implementation/Routing.md)

- REST urls contain only nouns
- Nouns map to collections of resources
- Individual resources can be accessed as part of a collection
- Resources can be nested under other resources

| Type | Url | Maps to |
|------|-----|---------|
| Collection | /api/ingredients | A collection of all ingredient resources |
| Resource | /api/ingredients/{ingredient} | The specific ingredient resource (e.g. /api/ingredient/flour) |
| Collection | /api/meals | A collection of all meal resources |
| Resource | /api/meals/{meal} | The specific meal resource (e.g. /api/meals/chili) |
| Nested collection | /api/meals/{meal}/ingredients | A collection of all ingredient resources of the specific meal |

## HTTP request methods

[Implementation guidelines](/docs/Implementation/Routing.md#API-verbs)

- Actions on the resources are defined by HTTP verbs
- The common verbs are GET, HEAD, POST, PUT, PATCH, and DELETE
- See [HTTP request methods](https://tools.ietf.org/html/rfc7231#section-4)
- Another good resource are the [Mozilla HTTP docs](https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods)

| Type | Example url | GET | HEAD | POST | PUT | PATCH | DELETE |
|------|-------------|-----|------|------|-----|-------|--------|
| Collection | /api/meals | Get collection of all meal resources | Same as GET, but only return headers | Create a new meal resource in the collection | Update (replace) batch of meals | _Not allowed_ | _Not allowed_ |
| Resource | /api/meals/chili | Get specific meal resource | Same as GET, but only return headers | _Not allowed_ | Update (replace) meal | Update parts of the meal resource | Delete the meal resource

HEAD returns the same header data as GET, but without any response body.
It is useful for things such as:

- Checking existence: Use `HEAD /api/meals/chili` to make a request and check if the response is 200 OK or 404 Not Found.
- Checking cache status: Make a HEAD request and check the header data if the response cache entry has expired yet or not.

## Safe and idempotent actions

- Some actions are defined to be safe.
  - Safe means that the client does not expect a server state change to occur because of the request.
- Some actions are defined to be idempotent.
  - Idempotency means that if the same request gets submitted multiple times the state of the resource is the same as if the request was submitted only once.
  - Idempotent actions are safe to retry, but non-idempotent are generally not safe to retry.

| Method | Safe | Idempotent |
|--------|------|------------|
| GET | yes | yes |
| HEAD | yes | yes |
| POST | no | no |
| PUT | no | yes |
| PATCH | no | no |
| DELETE | no | yes |

## Responses for HTTP requests

[Implementation guidelines](/docs/Implementation/Responses.md)

HTTP actions have standard response codes that should be followed.
This includes setting the HHTP response code correctly, as well as returning the expected content in the response body and potentially returning specific headers.
The following table shows the standard responses for common requests.

| HTTP action | Scenario | Return message code | Return body |
|-------------|----------|---------------------|-------------|
| GET | Resource found | 200 OK | Resource |
| GET | Resource not found | 404 Not Found | -empty- |
| POST | Resource created successfully | 201 Created | Resource (optional), Location header with link to the new resource |
| POST | Resource already exists | 409 Conflict | -empty- |
| PUT | Resource updated | 200 OK / 204 No Content | Resource / -empty- |
| PATCH | Resource updated | 200 OK / 204 No Content | Resource / -empty- |
| DELETE | Resource deleted | 200 OK / 204 No Content | Deletion details / -empty- |
| DELETE | Resource not found / already deleted | 404 Not Found | -empty- |

## API Versioning

[Implementation guidelines](/docs/Implementation/Routing.md)

Always version your API.
Even if you think there will never be another version.

Common versioning schemes include:

- Route based versioning: `/api/v1/meals`, `/api/v2/meals`
- Query-string versioning: `/api/meals?api-version=1.0`
- Header based versioning: Include headers in the request to specify the API version

Route based versioning is quite common, because it makes the version explicit (you know which version is called even without needing to know query paratemeters or headers) and because it makes it easy to cleanly separate code for different versions.

## OpenAPI documentation for API discovery

[Implementation guidelines](/docs/Implementation/OpenAPI.md)

[OpenAPI](https://www.openapis.org/about) offers a way for both people and services to discover APIs and learn what functionalities the API offers.
The main parts are:

- API discovery endpoint that returns a json which describes the API functionality, that is all the available API endpoints and what the input and output formats are. This can be used by services communicating with the API to create class models that fit the API specification.
- An API documentation page (built on top the of the aforementioned json) that can be used by people to understand the API. The documentation page includes descriptions of all the methods and paramters (if you documented your code) and can have "try out" sections for people to make requests and see response formats.

One of the most popular tools for creating OpenAPI documentation is Swagger.

Difference between OpenAPI and Swagger? Check the [Swagger blog](https://swagger.io/blog/api-strategy/difference-between-swagger-and-openapi/).

- OpenAPI = Specification
- Swagger = Tools for implementing the specification

## Caching

[Implementation guidelines](/docs/Implementation/Caching.md)

See details about caching in the [ASP.NET CORE docs](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-2.2).
To quote the docs:
> Under the official specification, caching is meant to reduce the latency and network overhead of satisfying requests across a network of clients, proxies, and servers. It isn't necessarily a way to control the load on an origin server.

Cache data if:

- Returned data is the same for anyone who requests the data (data requires no personal information)
- Queries don't require authentication
- Usually only GET or HEAD requests that return a 200 status code are cached

To achieve the most benefits from caching both of the following methods should be used:

- ETags
- Cache-Control

### ETags

The server returns ETag headers for each response, which can be saved by the client.
The ETag will remain the same for identical requests as long as the resource hasn't changed.

There are two options for ETags:

- **Strong ETag** (e.g. ETag: "123456789"): A strong ETag means a response with the same ETag is byte-for-byte identical. This means both the response body and response headers match.
- **Weak ETag** (e.g. ETag: W/"123456789"): A weak ETag means a response with the same ETag is semtically equivalent. This means the response body matches, but the response headers are not considered.

Assume a client has requested a resource and received it with the ETag "123456789". ETags can then be used for the following two purposes.

| Purpose | Header | Scenario |
| ------- | ------ | -------- |
| Validation | If-None-Match | Clients can add this header to a GET request (e.g. If-None-Match: "123456789"). If the resource on the server or cache matches the Etag a 304 Not Modified response is returned, otherwise the changed resource is returned along with a new ETag. |
| Concurrency checks | If-Match | Clients can add this header (e.g. If-Match: "123456789") to avoid race conditions for modification requests (e.g. two concurrent PUT requests for the same resource). If the provided ETag matches the ETag of the resource the request is accepted and the resource is updated (which also changes its ETag), otherwise if the ETags don't match a 412 Precondition Failed response is returned. In the latter case the client has to get the resource again (which will return the updated resource and ETag) and then modify the resource and attempt the PUT or PATCH request again with the new ETag. |

The ETag scenarios are summarized in the following table.

| Request | Header | Scenario | Response |
| ------- | ------ | -------- | -------- |
| GET | If-None-Match | Resource on server or cache matches ETag. | 304 Not Modified, no response body |
| GET | If-None-Match| Resource has changed and doesn't match ETag. | 200 OK, updated resource in body, new ETag value in header |
| PUT / PATCH | If-Match | Resource on server matches ETag. Request is accepted and resource is updated. | 200 OK / 204 No Content, new ETag value in header |
| PUT / PATCH | If-Match | Resource doesn't match ETag. Request is rejected. | 412 Precondition Failed |

See [implementation guidelines](/docs/Implementation/Caching.md#ETags) for ETags.

### Cache-Control

Cache-Control headers allow for configuring if resources should be cached and for how long.
They can be added to both request and response headers.
See [KeyCDN](https://www.keycdn.com/blog/http-cache-headers) and [RFC](https://tools.ietf.org/html/rfc7234#section-5.2) docs for an overview of Cache-Control.

#### Response headers

If a cached entry is returned, an **Age** header is included which signifies the current age of the entry in seconds.
The response can also include a **Cache-Control** header with any of the following properties:

| Header | Meaning |
| ------ | ------- |
| max-age | The maximum number of seconds after which the entry will be considered stale. |
| s-maxage | In shared caches, this overrides the **max-age** value. |
| public | The entry may be cached. |
| private | The entry must not be cached in a public cache (a cache used by multiple users), but may be cached in a private cache (a cache used by a single user, e.g. local cache in a smartphone app). |
| no-cache | The response can't be returned from a cache entry without first validating the entry against the server (e.g. using ETags). |
| must-revalidate | The entry can be cached, but if the cached entry becomes stale it first has to be validated against the server before it can be used again. A validation will either reset the freshness of an unchanged entry, or update the entry with any changes and also reset its freshenss. |
| proxy-revalidate | Same as **must-revalidate** but does not apply to private caches. |
| no-store | The response must not be cached. Used if response contains private information that should not be stored. |
| no-transform | The response must not be transformed (e.g. image compression, media-type conversion). |

#### Request headers

Request headers can be sent by clients to control if cached responses should be returned, or if responses should be added to a cache.

| Header | Meaning |
| ------ | ------- |
| max-age | The client only allows cached responses with an age in seconds lower than the specified max age. If the server cache is older, the server has to recreate the response. The cache is then refreshed with the new response. |
| min-fresh | The client only wants a cached response if it will stay fresh for at least the specified number of seconds before it goes stale. |
| max-stale | The client is willing to accept a stale cached response that has been stale for no longer than the specified number of seconds. |
| no-cache | The client wants the server to ignore the cache even if the response is already cached. The server then serves a new fresh result and updates the cache with it, resetting the cache age. |
| no-store | The client wants the server to not store the response in any cache. If the response is already cached, the cached result is returned. |
| only-if-cached | The client will only accept a cached response. Usually used when network connections are bad. |

The **ResponseCaching** middleware is an example of a middleware that handles Cache-Control headers, implements a cache, and automatically performs the caching of responses.
Using the middleware means that no code changes besides some setup code is required to add caching to an API.

See [implementation guidelines](/docs/Implementation/Caching.md#Caching-implementation-using-ResponseCaching-middleware) for ResponseCaching.

### Cache Types

There are different options for caches.

| Location | Option | Scenario |
| -------- | ------ | -------- |
| Server side | In-Memory caching | Use this when you only have single server or multiple servers with server affinity turned on (sticky sessions). While this is easy to implement, most services will run on more than on server and adding server affinity adds complexity in other areas. So overall this is not recommended for most production services. |
| Server side |  Distributed caching | Preferred over In-Memory caching. Use a distributed cache that each of the servers talks to. Keeps cache in sync between all instances, so server affinity is not needed. Distributed caches can be based on Redis, SQL, or others. |

## Rate limiting

[Implementation guidelines](/docs/Implementation/RateLimiting.md)

Rate limiting is used to reject requests from a client that has already made too many requests in a given timeframe.

- This can be used to prevent deterioration of the service due to malicious clients making too many requests.
- It can also be used to define usage quotas, such as for different account tiers. For example, a free account may only be allowed to make 100 requests a day, whereas a paid account may be allowed 10000 requests a day.

In general, rate limiting adds a set of headers to responses that alert the client of the remaining usage quota. If the quota should be exceeded the request is rejected with a **429 Too Many Requests** response.

## Use Data Transfer Objects (DTOs)

For more details about DTOs see the [ASP.NET Web API docs](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5).

An onion layer model fits well for APIs.
In an onion model several different layers are built on top of each other, where the inner layers are independent of the outer layers.
The domain layer that contains the domain logic for the application is the innermost layer.
One of the benefits of an onion model is that the domain layer logic can be tested and developed without needing to deal with application details, such as database connections, which live in outer layers.

In practice it leads to clean separation of the layers when splitting the different layers into separate projects.

| Layer | Type |  Usage |
| ----- | ---- |  ----- |
| Domain layer (innermost layer) | Domain model | Used when performing the domain logic. All actions should be done on domain models. |
| Framework layer | Persistence model | Used to save and retrieve domain models from a persistence layer (SQL, no-SQL, etc.). Domain models are converted to and from persistence models. Consumes domain layer. |
| API layer (outermost layer) | DTO | Used for inputs and outputs of API method. Convert domain models into appropriate DTOs which contain only the needed information. If the domain model contains more information than is necessary, then those fields can be left out in the DTO. Consumes framework layer and domain layer. |

### Domain models

- Avoid anemic models which consist only of properties and no methods
- Avoid base data types (int, string, etc.) for properties. Encapsulate properties into classes.
  - For example, if a user class has a first name propery, don't use a string. Instead create a FirstName class that can check if a string is a valid name.
  - This will confine all validation of the poperty to the property class, and all other parts of the code can use the property and be guaranteed that it is valid.

### Data persistence models

- Defined outside of the domain model layer in a different project that deals with getting and storing domain models.
- Domain models are converted to separate data persistence objects.
  - Persistence objects are specific to the data persistence technology (SQL, no-sql, etc.). The technology defines how the persistence objects have to look like.
  - By having the persistence models in separate projects, the domain logic remains unaffected if the persistaence layer is changed from one technology to another.

### DTOs

- Use POCOs (Plain-old CLR objects) which only have properties using base data types.
  - Avoid complex data types. DTOs should be easily serializable.
  - Translate complex domain models by squashing complex data types into several properties using base data types.
    - For example, if a class has a property `public FullName Name` where FullName is a complex type this could be converted to two properties `string FirstName` and `string LastName`.
- DTOs are usually anemic models
- Use a different DTO for each scenario and select only the relevant domain model properties that are needed.
  - For example, a meal domain object may have a separate DTO for creating, for updating, and for viewing the meal data.