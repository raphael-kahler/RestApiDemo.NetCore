# Overview

This repo contains a demo REST API that shows the basic structure and functionality and API should contain when following a RESTful pattern.
The API is built in ASP<span></span>.NET Core 2.2.
See [ASP.NET Core API docs](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.2) for a good getting started guide.

## REST overview

Check out the cheatsheet for API guidelines and implementation details:

- [REST API Cheatsheet](docs/REST-API-Cheatsheet.md)

## Running the API locally

1. Have Visual Studio installed (2017 or later)
2. Add the "ASP<span></span>.NET and web development tools" to Visual Studio using the Visual Studio Installer
3. Install the [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks) to build and run the API correctly
4. Clone or download the repo
5. Open the `RestApiDemo.NetCore.sln` in Visual Studio
6. Set the startup project as `RestApiDemo.API`
7. Run the project with profile "IIS Express"
8. Allow VS to create a self-signed cert and trust it. This will allow localhost https calls to the API.

The start page of the API is set to the Swagger API documentation page.
You can explore this page and make API calls from it.

You can also make REST calls from console or using other tools.
Check out some of the [example API calls](Example-API-Calls.http).