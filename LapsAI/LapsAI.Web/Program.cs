using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using LapsAI.Shared.Services;
using LapsAI.Web.Components;
using LapsAI.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
