using System.Net;
using FinancialAccounts.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounts.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception: {}", e);
            await HandleExceptionAsync(httpContext, e);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ClientNotFoundException => (int) HttpStatusCode.NotFound,
            DbUpdateException => (int) HttpStatusCode.Forbidden,
            _ => (int) HttpStatusCode.InternalServerError
        };

        await context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }
}
