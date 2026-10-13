using System.Diagnostics;

namespace FoodDelivery.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch(Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred while processing {Path}",
                    context.Request.Path
                    );

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "An unexpected error occured.",
                    Data = (object?)null,
                    TraceId = Activity.Current?.Id
                        ?? context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
