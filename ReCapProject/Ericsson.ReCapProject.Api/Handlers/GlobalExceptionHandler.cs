using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Ericsson.ReCapProject.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = exception switch
            {
                ApplicationException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };
            var result = JsonSerializer.Serialize(new { code = response.StatusCode, message = exception?.Message });
            await response.WriteAsync(result, cancellationToken);

            return true;
        }
    }
}