
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace SFCoreProTM.Presentation.Middlewares
{
    /// <summary>
    /// Middleware global untuk menangani semua exception yang tidak tertangani dalam aplikasi.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred while processing the request for {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var responseModel = new ErrorResponseModel();

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = string.IsNullOrWhiteSpace(validationEx.Message)
                        ? "Request failed due to validation errors."
                        : validationEx.Message;

                    var failures = validationEx.Errors ?? Enumerable.Empty<ValidationFailure>();
                    var groupedFailures = failures
                        .Where(failure => failure is not null)
                        .GroupBy(
                            failure => string.IsNullOrWhiteSpace(failure.PropertyName) ? "_global" : failure.PropertyName,
                            failure => failure,
                            StringComparer.OrdinalIgnoreCase);

                    var validationErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                    foreach (var group in groupedFailures)
                    {
                        var messages = group
                            .Select(failure => failure.ErrorMessage)
                            .Where(message => !string.IsNullOrWhiteSpace(message))
                            .Distinct()
                            .ToArray();

                        if (messages.Length > 0)
                        {
                            validationErrors[group.Key] = messages;
                        }
                    }

                    if (validationErrors.Count > 0)
                    {
                        responseModel.Errors = validationErrors;
                    }

                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                    responseModel.Message = notFoundEx.Message;
                    break;

                case DomainException domainEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = string.IsNullOrWhiteSpace(domainEx.Message)
                        ? "A domain rule was violated."
                        : domainEx.Message;

                    if (domainEx.Errors.Count > 0)
                    {
                        responseModel.Errors = domainEx.Errors.ToDictionary(
                            pair => pair.Key,
                            pair => pair.Value,
                            StringComparer.OrdinalIgnoreCase);
                    }

                    break;

                case ConflictException conflictEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    responseModel.StatusCode = (int)HttpStatusCode.Conflict;
                    responseModel.Message = string.IsNullOrWhiteSpace(conflictEx.Message)
                        ? "The requested operation resulted in a conflict."
                        : conflictEx.Message;
                    break;

                case AuthenticationException authEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseModel.Message = string.IsNullOrWhiteSpace(authEx.Message)
                        ? "Invalid credentials provided."
                        : authEx.Message;
                    break;

                case UnauthorizedAccessException _:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseModel.Message = "You are not authorized to perform this action.";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Message = "An unexpected internal server error occurred. Please try again later.";
                    break;
            }

            // Hanya sertakan detail error (stack trace) di environment Development
            if (_env.IsDevelopment())
            {
                responseModel.Details = exception.ToString();
            }

            var result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Model respons error yang standar untuk API.
    /// </summary>
    public class ErrorResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
        public string? Details { get; set; }
    }

    /// <summary>
    /// Extension method untuk mendaftarkan GlobalExceptionHandlerMiddleware.
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
