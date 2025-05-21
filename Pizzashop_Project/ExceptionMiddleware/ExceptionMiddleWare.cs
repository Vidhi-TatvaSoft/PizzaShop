using System.Net;
using System.Text.Json;

namespace Pizzashop_Project.ExceptionMiddleware;

public class ExceptionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleWare> _logger;
    public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptions(context, ex);
        }
    }

    private async Task HandleExceptions(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string message;

                code = HttpStatusCode.InternalServerError;
                message = "Something went wrong. Please try again later.";

        _logger.LogError(exception, "An unhandled exception occurred.");

        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        
        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // Always OK (to avoid redirect issues)

            context.Response.Headers.Add("X-Error", "true");

            var jsonResponse = new
            {
                success = false,
                statusCode = (int)code,
                error = message
            };
            
            string jsonMessage = JsonSerializer.Serialize(jsonResponse);
            await context.Response.WriteAsync(jsonMessage);
        }
        else
        {
            // // For Normal Requests - use TempData for Toastr
            if (!context.Response.HasStarted)
            {
                var redirectUrl = $"/ErrorPage/InternalServerError";
                context.Response.StatusCode = (int)HttpStatusCode.Redirect; // 302
                context.Response.Headers["Location"] = redirectUrl; // Manual redirect
                await context.Response.CompleteAsync();
            }
            else
            {
                _logger.LogWarning("Response already started, cannot redirect.");
                context.Response.StatusCode = (int)code;
                await context.Response.WriteAsync(message);
            }
        }
    }
}


