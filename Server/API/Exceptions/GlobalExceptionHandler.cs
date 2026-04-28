using FluentValidation;

namespace Server.API.Exceptions;

public record ExceptionResponse(
    int StatusCode,
    string Type,
    string Detail
);

public sealed class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing the request.");

            context.Response.StatusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                InternalServerException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(
                new ExceptionResponse(
                    StatusCode: context.Response.StatusCode,
                    Type: ex.GetType().Name,
                    Detail: ex.Message
                )
            );
        }
    }
}