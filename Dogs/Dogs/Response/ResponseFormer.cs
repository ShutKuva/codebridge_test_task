using System.Net.Mime;

namespace Dogs.Response
{
    public static class ResponseFormer
    {
        public static async Task<HttpContext> WriteResponse<T>(this HttpContext context, int statusCode, T responseBody)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsJsonAsync(responseBody);
            return context;
        }
    }
}
