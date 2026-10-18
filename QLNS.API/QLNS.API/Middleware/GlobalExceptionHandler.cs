using System.Net;
using System.Text.Json;

namespace QLNS.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message, errorCode) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, "NOT_FOUND"),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message, "BAD_REQUEST"),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message, "INVALID_OPERATION"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "UNAUTHORIZED"),
                _ => (HttpStatusCode.InternalServerError, "An error occurred. Please try again later.", "INTERNAL_SERVER_ERROR")
            };

            response.StatusCode = (int)statusCode;

            var payload = QLNSCore.Entities.Result<object>.Failure(
                message: message,
                errors: new List<string> { exception.Message },
                errorCode: errorCode
            );

            var result = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }
}