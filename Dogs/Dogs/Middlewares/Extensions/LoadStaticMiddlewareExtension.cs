using Adapters.Abstractions;

namespace Dogs.Middlewares.Extensions
{
    public static class LoadStaticMiddlewareExtension
    {
        public static IApplicationBuilder UseLoadStatic(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<IDogsAdapter>();
            }
            return app;
        }
    }
}
