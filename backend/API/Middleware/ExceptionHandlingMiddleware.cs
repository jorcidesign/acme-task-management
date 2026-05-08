using System.Net;
using System.Text.Json;
using AcmeTaskApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
 
namespace AcmeTaskApi.API.Middleware;
 
/// <summary>
/// Global exception handler middleware.
/// Converts domain exceptions into RFC 7807 ProblemDetails responses.
/// Keeps controllers clean — no try/catch blocks needed.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
 
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }
 
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException ex        => (HttpStatusCode.NotFound, "Resource Not Found", ex.Message),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, "Authentication Failed", "The credentials provided are invalid."),
            ForbiddenException          => (HttpStatusCode.Forbidden, "Access Denied", "You do not have permission to perform this action."),
            _                           => (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.")
        };
 
        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
 
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
 
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}