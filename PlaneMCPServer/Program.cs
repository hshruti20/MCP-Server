using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration
.SetBasePath(AppContext.BaseDirectory)
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>()
.AddEnvironmentVariables();

var planeApiKey = builder.Configuration["PlaneAPIKey"];
var baseUrl = builder.Configuration["BaseUrl"];
var workspace = builder.Configuration["Workspace"];
var projectId = builder.Configuration["ProjectId"];

if (string.IsNullOrEmpty(planeApiKey))
    throw new InvalidOperationException("Plane API key is not set. Please set the PlaneAPIKey in user secrets.");
if (string.IsNullOrEmpty(baseUrl))
    throw new InvalidOperationException("BaseUrl key is not set. Please set the BaseUrl in user secrets.");
if (string.IsNullOrEmpty(workspace))
    throw new InvalidOperationException("Workspace key is not set. Please set the Workspace in user secrets.");
if (string.IsNullOrEmpty(projectId))
    throw new InvalidOperationException("ProjectId key is not set. Please set the ProjectId in user secrets.");

builder.Services.AddHttpClient();
builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new PlaneApiService(httpClientFactory,baseUrl,workspace,projectId,planeApiKey);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();




