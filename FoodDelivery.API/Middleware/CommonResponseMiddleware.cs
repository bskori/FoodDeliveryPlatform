using System.Text.Json;

namespace FoodDelivery.API.Middleware
{
    public class CommonResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public CommonResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/items"))
            {
                await _next(context);
                return;
            }

            var originalBody = context.Response.Body;

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                memoryStream.Position = 0;

                var statusCode = context.Response.StatusCode;
                var contentType = context.Response.ContentType;

                bool isJson = contentType != null &&
                    (contentType.Contains(
                        "application/json",
                        StringComparison.OrdinalIgnoreCase) ||
                        contentType.Contains(
                            "+json",
                            StringComparison.OrdinalIgnoreCase));
                if(!isJson || 
                    memoryStream.Length == 0 ||
                    statusCode == StatusCodes.Status204NoContent)
                {
                    context.Response.Body = originalBody;
                    await memoryStream.CopyToAsync(originalBody);
                    return;
                }

                using var document = await JsonDocument.ParseAsync(memoryStream);

                var responseData = document.RootElement.Clone();

                bool success = statusCode >= 200 && statusCode < 300;

                string message = statusCode switch
                {
                    200 => "Request completed succesfully.",
                    400 => "Invalid request.",
                    404 => "Resource not found.",
                    _ => success ? "Request completed succesfully." : "Request failed."
                };

                var response = new
                {
                    Success = success,
                    StatusCode = statusCode,
                    Message = message,
                    Data = responseData
                };

                context.Response.Body = originalBody;
                context.Response.ContentLength = null;

                await context.Response.WriteAsJsonAsync(response);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
