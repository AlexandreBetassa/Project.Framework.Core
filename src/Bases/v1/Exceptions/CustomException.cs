using System.Net;

namespace Store.Framework.Core.Bases.v1.Exceptions
{
    public class CustomException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public int StatusCode { get; set; } = (int)statusCode;
    }
}