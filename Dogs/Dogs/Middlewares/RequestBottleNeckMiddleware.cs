using Core;
using Core.Cache;
using Dogs.Abstractions;
using Dogs.Response;
using Microsoft.Extensions.Options;

namespace Dogs.Middlewares
{
    public class RequestBottleNeckMiddleware
    {
        private const string REQUEST_COUNTER = "RequestCounter";

        private readonly RequestDelegate _next;
        private readonly ICache<string> _cache;
        private readonly ServerConfiguration _serverConfiguration;

        public RequestBottleNeckMiddleware(RequestDelegate next, ICache<string> cache, IOptions<ServerConfiguration> serverConfiguration)
        {
            _next = next;
            _cache = cache;
            _serverConfiguration = serverConfiguration.Value ?? throw new ArgumentNullException(nameof(serverConfiguration));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            RequestsInformation information = await _cache.GetAsync(REQUEST_COUNTER, new RequestsInformation());

            if (information.NumberOfRequests == -1)
            {
                information.NumberOfRequests = 0;
                await _cache.AddAsync(REQUEST_COUNTER, information);
            }

            if (information.LastRequest >= DateTime.Now && information.NumberOfRequests < _serverConfiguration.MaxRequests)
            {
                await _cache.SetAsync(REQUEST_COUNTER, new RequestsInformation
                {
                    NumberOfRequests = information.NumberOfRequests + 1,
                    LastRequest = information.LastRequest,
                });

                Console.WriteLine($"Requests within second: {information.NumberOfRequests + 1}");

                await _next(context);
            } else if (information.LastRequest < DateTime.Now)
            {
                await _cache.SetAsync(REQUEST_COUNTER, new RequestsInformation
                {
                    NumberOfRequests = 1,
                    LastRequest = DateTime.Now.AddSeconds(_serverConfiguration.TimeWindowInSeconds)
                });
                Console.WriteLine($"Time changed");
                await _next(context);
            }
            else
            {
                await context.WriteResponse(StatusCodes.Status429TooManyRequests, new Error(StatusCodes.Status429TooManyRequests, "To many requests."));
            }
        }
    }
}
