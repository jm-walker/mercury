using Mercury.Api;
using Mercury.Api.Logic;
using Mercury.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.XPath;

var builder = WebApplication.CreateBuilder(args);

// Configuration values
builder.Configuration
    .AddJsonFile("appsettings.json", false)
    .AddEnvironmentVariables();

// Setup API and Routing
var DEFAULT_VERSION = new ApiVersion(1, 0);

// Versioned endpoints
builder.Services
    .AddEndpointsApiExplorer()
    .AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = DEFAULT_VERSION;
    })
    .AddApiVersioning(options =>
    {

        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = DEFAULT_VERSION;
        options.ReportApiVersions = true;
    });

// Load Services
builder.Services.Configure<ApiConfig>(builder.Configuration.GetRequiredSection(nameof(ApiConfig)));

// Swagger generator
builder.Services.AddSwaggerGen( setup =>
{
    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();


    var section = config.GetSection(nameof(ApiConfig)).Get<ApiConfig>();

    string xml = File.ReadAllText( Path.Combine(AppContext.BaseDirectory, "Mercury.Api.xml"));

    xml = xml.Replace("[[DEFAULT_SERVICES]]", String.Join(',', section.DefaultServices))
            .Replace("[[ALL_SERVICES]]", String.Join(',', section.AllowedServices));
    using (var sr = new StringReader(xml))
    {
        XPathDocument doc = new XPathDocument(sr);
        setup.IncludeXmlComments(() => doc, true);
    }

    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Mercury.Common.xml"));
    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Mercury.Plugin.xml"));
});
// Force enums to use strings
builder.Services.AddControllers()
       .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


// Add DI Services
builder.Services
    .AddScoped<IJobHandler, JobHandler>()
    .AddSingleton<IJobPersist, JobPersist>()
    .AddSingleton<IMessageBroker, Broker>();

// Redis Services
builder.Services.Configure<RedisConfiguration>(
        builder.Configuration.GetRequiredSection("Redis")
);

builder.Services
        .AddSingleton((ctx) =>
        {
            return ctx.GetRequiredService<IOptions<RedisConfiguration>>().Value;
        });

builder.Services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>((ctx) => new List<RedisConfiguration> { ctx.GetRequiredService<IOptions<RedisConfiguration>>().Value });

// MQ Services
builder.Services.Configure<MessageBrokerConfig>(
        builder.Configuration.GetRequiredSection("MQ")
);
builder.Services
        .AddSingleton<IConnectionFactory, ConnectionFactory>((ctx) =>
         {
             var config = ctx.GetRequiredService<IOptions<MessageBrokerConfig>>().Value;
             
             return new ConnectionFactory() { HostName = config.Hostname, UserName = config.Username, Password = config.Password, Port = config.Port };
         });

// Logging
builder.Services.AddLogging(ctx => ctx.AddSimpleConsole( opts =>
{
    opts.IncludeScopes = true;
    opts.SingleLine = true;
    opts.TimestampFormat = "hh:mm:ss | ";
}));


// Build everything
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Output config if debugging
foreach( var kvp in app.Configuration.AsEnumerable())
{
    if( kvp.Key.ToUpperInvariant().Contains("PASS"))
    {
        app.Logger.LogDebug(" CONFIG | {0} = ****", kvp.Key);
    }
    app.Logger.LogDebug(" CONFIG | {0} = {1}", kvp.Key, kvp.Value);
}


app.MapControllers();

// Go
app.Run();
