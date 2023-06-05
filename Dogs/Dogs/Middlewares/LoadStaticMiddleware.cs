using Adapters.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Dogs.Middlewares
{
    public class LoadStaticMiddleware
    {
        private readonly RequestDelegate _next;

        public LoadStaticMiddleware(RequestDelegate next, IDogsAdapter adapter)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            return _next(context);
        }
    }
}
