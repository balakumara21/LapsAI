using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using FileManagerAI.Services;
using LapsAI.Shared;
using LapsAI.Shared.Services;
using LapsAI.Web.Components;
using LapsAI.Web.Services;
using Microsoft.Extensions.AI;
using OpenAI;
using SmartComponents.LocalEmbeddings;
using Syncfusion.Blazor;
using Syncfusion.Blazor.AI;
using Syncfusion.Blazor.SmartComponents;
using SyncfusionAISamples.Service;
using System.Net.Http;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF1cXGFCf1FpR2RGfV5ycUVHal9QTnRfUiweQnxTdEBiWHxecXVUT2FeWEZ0WkleYg==");

builder.Services.AddSyncfusionBlazor();
#region AI Integration
builder.Services.AddScoped<FileManagerService>();

//For PDF viewer
builder.Services.AddMemoryCache();
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 1024000000000; o.EnableDetailedErrors = true; });


// Local Embeddings
builder.Services.AddSingleton<LocalEmbedder>();


builder.Services.AddSyncfusionSmartComponents().InjectOpenAIInference();
// Open AI Service
string apiKey = "Api Key";
string deploymentName = "model-name";

OpenAIClient openAIClient = new OpenAIClient(apiKey);
IChatClient openAiChatClient = openAIClient.GetChatClient(deploymentName).AsIChatClient();
builder.Services.AddChatClient(openAiChatClient);


builder.Services.AddSingleton<SyncfusionAIService>();
builder.Services.AddSingleton<AzureAIService>();
//End

// Add device-specific services used by the LapsAI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddCorrelationId();


builder.Services.AddScoped<StorageService>();

builder.Services.AddTransient<TokenHandler>();


builder.Services.AddHttpClient<IAPIGateway, APIGateway>(httpclient =>
{

    httpclient.BaseAddress = new Uri(configuration["APIGatewayBaseURL"]);


}).AddCorrelationIdForwarding().AddHttpMessageHandler<TokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(LapsAI.Shared._Imports).Assembly);

app.Run();
#endregion