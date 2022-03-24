using System;
using System.Net;

namespace Camunda.Worker.Client;

public class ClientException : Exception
{
    private readonly ErrorResponse _errorResponse;

    public ClientException(ErrorResponse errorResponse, HttpStatusCode statusCode)
    {
        _errorResponse = errorResponse;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string ErrorType => _errorResponse.Type;
    public string ErrorMessage => _errorResponse.Message;

    public override string Message => $"Camunda error of type \"{ErrorType}\" with message \"{ErrorMessage}\"";
}
