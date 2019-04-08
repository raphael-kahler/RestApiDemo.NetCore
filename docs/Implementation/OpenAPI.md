# OpenAPI documentation

## Implementing OpenAPI documentation

**Note:** This section assumes that you are using versioned APIs as described in the [versioning section](Routing.md).

See [getting started documentation](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio) on how to use Swagger.

First, enable the creation of an XML documentation file as part of build.
This xml documentation file will be generated from the xml comments on each of the methods and classes.
The Swagger tools will read the XML documentation file to add the documented method and object details to the API documentation page.

In the .csproj file add the following entry ([see docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio#xml-comments)).

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

The NuGet packages that can be used to generate OpenAPI pages with Swagger are:

- `Swashbuckle.AspNetCore.Swagger`
- `Swashbuckle.AspNetCore.SwaggerGen`
- `Swashbuckle.AspNetCore.SwaggerUI`

Install the NuGet packages, then create the following class `ConfigureSwaggerOptions`.
This is not strictly necessary, but because Swagger configuration can get quite lengthy it can be nice to split it into a separate class.

```csharp
internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
        this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new Info()
                {
                    Title = $"[Your API name here] API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                });
        }

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: false);
    }
}
```

The `xmlPath` in the options will point to the generated XML documentation file that was enabled above in the .csproj file.

Next add the following to `ConfigureServices()` in Setup.cs

```csharp
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.AddSwaggerGen();
```

Also add the following to `Configure()` in Setup.cs

```csharp
app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = string.Empty; // show the Swagger UI as the app root page
    });
```

Setting the RoutePrefix to an empty string will make the Swagger API documentation page show up at the root index.html page for your API.
This can be quite nice since it makes the Swagger page easily discoverable, and it's likely your API will not have any other web pages besides the Swagger page.

With this the OpenAPI documentation will be generated as part of building the project.

- You can browse to the API root (e.g. `https://localhost:44376/index.html` or just `https://localhost:44376/`) and view the Swagger page.
- You can find the OpenAPI json at `https://localhost:44376/swagger/v1/swagger.json`. Replace v1 with the other API versions you might have.