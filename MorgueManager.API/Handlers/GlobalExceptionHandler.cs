using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MorgueManager.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MorgueManager.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string title;
        string detail;
        object? errors = null;

        if (exception is AppException appException)
        {
            statusCode = appException.StatusCode;
            title = appException.GetType().Name;
            detail = appException.Message;

            if (appException is AppValidationException valException)
            {
                errors = valException.Errors;
            }

            _logger.LogWarning("Ngoại lệ ứng dụng xảy ra (HTTP {StatusCode}): {Message}", 
                statusCode, exception.Message);
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            title = "InternalServerError";
            detail = "Một lỗi hệ thống nghiêm trọng đã xảy ra trên máy chủ.";
            
            // Log full exception stack trace for 500 errors
            _logger.LogError(exception, "Lỗi máy chủ không được xử lý (HTTP 500): {Message}", 
                exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (errors != null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
