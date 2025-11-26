
using LapsAIApiGateWay.Services;
using LapsAIDAO.Repository;
using LapsAIDAO.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTransient<IRepository, Repository>();

builder.Services.AddTransient<ILoginService, LoginService>();

builder.Services.AddSingleton<IDatabaseFactory>(sp =>
    DatabaseFactory.Create(
        builder.Configuration["Database:Type"],
        sp
    ));

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateWay", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                
                Reference = new OpenApiReference
                {

                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new System.Collections.Generic.List<string>()
        }
    });
});

// Register Ocelot
builder.Services.AddOcelot();

// Configure JWT Authentication using strongly-typed settings
var jwtSettings = new JwtSettings();
builder.Configuration.Bind("Jwt", jwtSettings);

if (string.IsNullOrEmpty(jwtSettings.Key) || string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience))
{
    throw new ArgumentNullException(nameof(jwtSettings), "JWT settings (Key, Issuer, Audience) must be configured in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// Register typed HttpClients for ServiceA and ServiceB
builder.Services.AddHttpClient<IServiceAClient, ServiceAClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5095");
});

builder.Services.AddHttpClient<IServiceBClient, ServiceBClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5092");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Simple sanity endpoint to verify routing is active
app.MapGet("/call/ping", () => Results.Ok("pong"));

app.MapControllers();

// Add Ocelot middleware under a specific path
app.Map("/gateway", ocelotApp =>
{
    ocelotApp.UseOcelot().Wait();
});

await app.RunAsync();

