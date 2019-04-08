using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;

namespace RestApiDemo.Api
{
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
                        Title = $"Meal API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                    });
            }

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: false);
        }
    }
}
