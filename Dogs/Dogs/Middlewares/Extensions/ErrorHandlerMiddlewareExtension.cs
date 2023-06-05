namespace Dogs.Middlewares.Extensions
{
    public static class ErrorHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
