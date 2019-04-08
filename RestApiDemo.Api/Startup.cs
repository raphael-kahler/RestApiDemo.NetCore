using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestApiDemo.Api.DTO.Converters;
using RestApiDemo.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace RestApiDemo.Api
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // use caching middleware, see: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-2.2
            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = false;
                options.MaximumBodySize = 64 * 1024 * 1024; // in bytes; largest cachable size for a single response body, default is 64 MB
                options.SizeLimit = 500 * 1024 * 1024; // in bytes; max size of the middleware cache, default is 100 MB
            });

            services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default",
                        new CacheProfile()
                        {
                            Duration = 60
                        });
                    options.CacheProfiles.Add("Never",
                        new CacheProfile()
                        {
                            Location = ResponseCacheLocation.None,
                            NoStore = true
                        });
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                // Remove null entries from json responses with the following line.
                .AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; });

            // Add API versioning services.
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true; // make help page show ~/api/v1/{controller}/ instead of ~/api/{version}/{controller}
                });

            // Add services to generate swagger page and API documentation.
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            // Inject url helpers using the current action context. The url helper is used to generate hyperlinks to other API calls.
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSingleton<IMealApplicationService, MealApplicationService>();
            services.AddTransient<DtoConverter, DtoConverter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // configure app to cache responses, see: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-2.2#configuration
            // note: The app.Use...() methods go in order and can short circuit later app.Use...() methods. Make sure UseResponseCaching() comes before UseMVC(), otherwise MVC will serve the results before the cache gets hit.
            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseMvc();

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
        }
    }
}
