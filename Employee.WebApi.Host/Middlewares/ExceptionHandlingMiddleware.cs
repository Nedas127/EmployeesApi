using Employee.WebApi.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Employee.WebApi.Host.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IOptions<JsonOptions> _serializerOptions;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IOptions<JsonOptions> serializerOptions)
    {
        _logger = logger;
        _next = next;
        _serializerOptions = serializerOptions;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ResourceNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode code)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Message = exception.Message
        }, _serializerOptions.Value.JsonSerializerOptions));
    }
}