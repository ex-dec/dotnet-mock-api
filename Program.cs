using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Konfigurasi Kestrel untuk menangani 100.000 concurrent connections
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 100_000;
    options.Limits.MaxConcurrentUpgradedConnections = 100_000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);

    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.UseConnectionLogging();
    });
});

var app = builder.Build();

app.MapGet("/success", async (HttpContext context) =>
{
    await SuccessHandler(context);
});

async Task SuccessHandler(HttpContext context)
{
    await WriteResponseMessage(context, StatusCodes.Status200OK, "Success!");
}

async Task WriteResponseMessage(HttpContext context, int status, string message)
{
    string response = JsonSerializer.Serialize(new { message });
    await WriteResponse(context, status, response);
}

async Task WriteResponse(HttpContext context, int status, string response)
{
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = status;

    await context.Response.WriteAsync(response);
}

app.Run();
