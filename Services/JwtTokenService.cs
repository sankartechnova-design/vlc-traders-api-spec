using System.Net;
using System.Text.Json;
using VLCTraders.Api.Models.Common;

namespace VLCTraders.Api.Middleware
{
    public class ExceptionHandlingMiddleware
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
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse
                {
                    Success = false,
                    Message = "Unexpected system error",
                    TraceId = context.TraceIdentifier
                };

                var payload = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
