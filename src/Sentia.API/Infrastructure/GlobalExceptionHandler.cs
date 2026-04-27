using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Common.Exceptions;

namespace Sentia.API.Infrastructure;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = exception switch
        {
            ValidationException ve => new ValidationProblemDetails(ve.Errors)
            {
                Status = statusCode,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred."
            },
            NotFoundException ne => new ProblemDetails
            {
                Status = statusCode,
                Title = "Resource Not Found",
                Detail = ne.Message
            },
            ForbiddenException fe => new ProblemDetails
            {
                Status = statusCode,
                Title = "Forbidden",
                Detail = fe.Message
            },
            _ => new ProblemDetails
            {
                Status = statusCode,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            }
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}