namespace Application.Common.Exceptions;
public class IdentityException : Exception
{
    public IdentityException() : base("Identity exception has occurred")
    {

    }

    public IdentityException(IEnumerable<string> errors) : base("Identity exception has occurred")
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; }
}