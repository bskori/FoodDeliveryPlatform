using System.Net.Http.Headers;

namespace FoodDelivery.MVC.Handlers
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //1. Get the current MVC http context
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            //2. Read the token from session
            string? token = null;

            if(httpContext != null)
            {
                token = httpContext.Session.GetString("AccessToken");
            }

            //3. Attach the token, if available
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            //4. Continue sending the HTTP request
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
