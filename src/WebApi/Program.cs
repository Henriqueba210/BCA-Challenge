using System.Diagnostics;

using Auction.Api.Common.Mapping;
using Auction.Api.Endpoints;
using Auction.Application;
using Auction.Infrastructure;

using AuctionServiceDefaults;

using Mapster;

using MapsterMapper;

using Microsoft.AspNetCore.Http.Features;

using OpenTelemetry.Resources;

using Scalar.AspNetCore;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, useInMemory: builder.Environment.IsEnvironment("Testing"))
    .AddOpenApi();


builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowAnyHeader();
}));

#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Auction"));

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
// scans the assembly and gets the IRegister, adding the registration to the TypeAdapterConfig
TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingRegistration).Assembly);
// register the mapper as Singleton service for my application
var mapperConfig = new Mapper(typeAdapterConfig);
builder.Services.AddSingleton<IMapper>(mapperConfig);


WebApplication app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Auction API")
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapDefaultEndpoints();

app.MapVehicleEndpoints();
app.MapAuctionEndpoints();
app.MapBidEndpoints();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCors();

await app.RunAsync();

namespace Auction.Api
{
    public abstract partial class Program;
}