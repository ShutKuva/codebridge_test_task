using Microsoft.AspNetCore.Http;

namespace Core
{
    public class Error
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public Error(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}