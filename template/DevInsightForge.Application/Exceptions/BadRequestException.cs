namespace DevInsightForge.Application.Exceptions;

/// <summary>
/// Represents a custom exception for bad requests in the application.
/// </summary>
[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException() : base("A bad request occurred.") { }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}


