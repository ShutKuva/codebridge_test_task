using Core;
using Dogs.Response;
using System.Net.Mime;

namespace Dogs.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch (Exception ex)
            {
                int statusCode = ex switch
                {
                    ArgumentException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError,
                };

                await context.WriteResponse(statusCode, new Error(statusCode, ex.Message));
            }
        }
    }
}
