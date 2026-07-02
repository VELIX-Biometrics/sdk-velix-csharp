namespace Velix.SDK.Exceptions;

public class AuthException : VelixException
{
    public AuthException(string message, string? errorCode = null)
        : base(message, 401, errorCode)
    {
    }
}
