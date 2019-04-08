# Return types for API controllers

Make the return type of API controller methods one of these options:

| Return type | Scenario |
|-------------|----------|
| `ActionResult<T>` | Preferred option. Whenever the API returns an http body, it will be of type T. But the API can also return other results like 404 NOT FOUND. |
| `IActionResult` | Use this instead of `ActionResult<T>` when there are multiple possible return types, or the return type is an interface like `IEnumerable<T>`. The controller needs extra attribute decoration in this case. |
| Specific type | Discouraged. The API always has to return the object and can't return any other results such as error results. |

Decorate each API controller method with the `ProducesResponseType` attribute.
Add one attribute for every HTTP result that the API can return.

```csharp
// Alternative 1: ActionResult<T>
// When returning ActionResult, the return type is automatically assumed
// in the ProducesResponseType attribute and doesn't have to be specified.
[Route("{id}")]
[HttpGet]
[ProducesResponseType(200)] // you can also use [ProducesResponseType((int)HttpStatusCode.OK)] for more readability
[ProducesResponseType(404)]
public ActionResult<Ingredient> Get(int id)
{
    if (!GetIngredient(id, out Ingredient ingredient))
    {
        return NotFound($"Meal with id \"{id}\" does not exist.");
    }
    return Ok(ingredient);
}

// Alternative 2: IActionResult
// When returning IActionResult, it is necessary to specify the return type
// in the ProducesResponseType attribute.
// Otherwise the code is the same in alternative 1.
[Route("{id}")]
[HttpGet]
[ProducesResponseType(200, Type = typeof(Ingredient))]
[ProducesResponseType(404)]
public IActionResult Get(int id)
{
    if (!GetIngredient(id, out Ingredient ingredient))
    {
        return NotFound($"Meal with id \"{id}\" does not exist.");
    }
    return Ok(ingredient);
}
```

Check [return types for API controllers](https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-2.2) for more details.