using Learniverse.API.Contracts.Responses;
using Learniverse.Application.Exceptions;
using Learniverse.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using ValidationException = Learniverse.Application.Exceptions.ValidationException;

namespace Learniverse.API.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            AuthenticationFailedException authenticationFailedEx => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = authenticationFailedEx.Message,
                    Errors = null
                }),
            UnauthorizedException unauthorizedEx => (
                   HttpStatusCode.Unauthorized,
                   new ErrorResponse
                   {
                       StatusCode = (int)HttpStatusCode.Unauthorized,
                       Message = unauthorizedEx.Message,
                       Errors = null
                   }),
            AccountLockedException accountLockedEx => (
                  HttpStatusCode.Locked,
                  new ErrorResponse
                  {
                      StatusCode = (int)HttpStatusCode.Locked,
                      Message = accountLockedEx.Message,
                      Errors = null
                  }),

            AccountNotAllowedException accountNotAllowedEx => (
                HttpStatusCode.Forbidden,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Message = accountNotAllowedEx.Message,
                    Errors = null
                }),

            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "One or more validation errors occurred.",
                    Errors = validationEx.Errors
                }),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = notFoundEx.Message,
                    Errors = null
                }),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Message = forbiddenEx.Message,
                    Errors = null
                }),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = conflictEx.Message,
                    Errors = null
                }),

            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = domainEx.Message,
                    Errors = null
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later.",
                    Errors = null
                })
        };

        if (exception is UnauthorizedException ||
    (int)statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "An unhandled or critical application exception occurred.");
        }
        else
        {
            _logger.LogWarning(
                exception,
                "A handled application exception occurred.");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}