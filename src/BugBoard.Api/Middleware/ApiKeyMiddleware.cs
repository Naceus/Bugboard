using BugBoard.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace BugBoard.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _service;

        public ApiKeyMiddleware(RequestDelegate next, IServiceProvider service)
        {
            _next = next;
            _service = service;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/api/agent"))
            {
                await _next(context);
                return;
            }

            var apiKeyValue = context.Request.Headers["X-Api-Key"].ToString();
            if (string.IsNullOrEmpty(apiKeyValue))
            {
                context.Response.StatusCode = 401;
                return;
            }

            using var scope = _service.CreateScope();
            var context_db = scope.ServiceProvider.GetRequiredService<BugBoardDbContext>();
            var apiKey = await context_db.ApiKeys.FirstOrDefaultAsync(k => k.Key == apiKeyValue);

            if (apiKey == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey.UserId)
            }, "ApiKey"));
            await _next(context);
        }
    }
}
