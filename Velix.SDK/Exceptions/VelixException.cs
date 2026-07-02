namespace Velix.SDK.Exceptions;

public class VelixException : Exception
{
    public int StatusCode { get; }
    public string? ErrorCode { get; }

    public VelixException(string message, int statusCode = 0, string? errorCode = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public VelixException(string message, int statusCode, string? errorCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
