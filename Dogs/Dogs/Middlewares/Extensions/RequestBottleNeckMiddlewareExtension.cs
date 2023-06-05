namespace Dogs.Middlewares.Extensions
{
    public static class RequestBottleNeckMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestBottleNeck(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestBottleNeckMiddleware>();
            return app;
        }
    }
}
