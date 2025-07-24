using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];

// Настройка OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("service-b")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("service-b"))
            .AddAspNetCoreInstrumentation() // Входящие запросы
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri(tracingOtlpEndpoint);
            });
    });

var app = builder.Build();

app.MapGet("/api", () =>
{
    return "Hello from service-b!";
});

app.Run();