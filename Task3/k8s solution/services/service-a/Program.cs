using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"] ?? "http://localhost:4317";
var otherServiceEndpoint = builder.Configuration["SERVICE_B_ENDPOINT_URL"] ?? "http://localhost:5002/api";

builder.Services.AddHttpClient();

var otel = builder.Services.AddOpenTelemetry()
                           .WithTracing(tracing =>
                            {
                                tracing.AddSource("service-a");
                                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("service-a"));
                                tracing.AddAspNetCoreInstrumentation();
                                tracing.AddHttpClientInstrumentation();
                                if (tracingOtlpEndpoint != null)
                                {
                                    tracing.AddOtlpExporter(otlpOptions =>
                                    {
                                        otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                                    });
                                }
                                else
                                {
                                    tracing.AddConsoleExporter();
                                }
                            });

var app = builder.Build();

app.MapGet("/call", async (HttpClient httpClient) =>
{
    var response = await httpClient.GetStringAsync(otherServiceEndpoint);
    return $"service-b ответил - {response}";
});

app.Run();
