using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
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

app.Run("http://0.0.0.0:8080");