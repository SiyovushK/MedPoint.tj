using System.Net;

namespace Domain.Responses;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public Response(T? data)
    {
        IsSuccess = true;
        Data = data;
        StatusCode = 200;
        Message = string.Empty;
    }

    public Response(HttpStatusCode statusCode, string message)
    {
        IsSuccess = true;
        Data = default;
        StatusCode = (int)statusCode;
        Message = message;
    }
}