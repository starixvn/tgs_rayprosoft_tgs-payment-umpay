using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Payment.UmPay;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("/config/appsettings.json", true);

builder.Services
    .AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.MediaTypeOptions.AddText("application/x-www-form-urlencoded");
        logging.MediaTypeOptions.AddText("text/html");
    });

builder.Services
    .AddUmPay((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        config.GetSection("UmPay").Bind(options);
    })
    .AddSingleton<UmPaySerializer>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddTransient(sp =>
{
    var options = sp.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

    return options;
});

builder.Services.AddHealthChecks();

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    });

builder.Services.AddOpenTelemetryTracing(builder => builder
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
    .AddEnvironmentVariableDetector())
    .AddSource("Tgs.*")
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()    
    .AddOtlpExporter());

builder.Services.Configure<AspNetCoreInstrumentationOptions>(options =>
{
    options.Filter = httpContext =>
    {
        return !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !new[] { "html", "ico", "png", "jpg" }.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
    };
});

builder.Host.ConfigureLogging(logging => logging.AddTgsLog());

var app = builder.Build();

app.UseHttpLogging();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.MapHealthChecks("/healthz");

app.Use(async (context, next) =>
{
    await next();

    using var stream = new StreamReader(context.Request.Body);

    _ = await stream.ReadToEndAsync();
});

app.Run();