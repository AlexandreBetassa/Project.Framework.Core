using System.Net;

namespace Store.Framework.Core.v1.Bases.Exceptions
{
    public class CustomException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public int StatusCode { get; set; } = (int)statusCode;
    }
}