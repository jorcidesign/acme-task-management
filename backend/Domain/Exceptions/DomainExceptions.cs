namespace AcmeTaskApi.Domain.Exceptions;
 
/// <summary>
/// Thrown when a requested resource does not exist (maps to HTTP 404).
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string resource, object key)
        : base($"Resource '{resource}' with identifier '{key}' was not found.") { }
}
 
/// <summary>
/// Thrown when authentication credentials are invalid (maps to HTTP 401).
/// </summary>
public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("The provided credentials are invalid.") { }
}
 
/// <summary>
/// Thrown when a user attempts to access a resource they do not own (maps to HTTP 403).
/// </summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException()
        : base("You do not have permission to access this resource.") { }
}