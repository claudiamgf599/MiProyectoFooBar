namespace FooBar.Domain.Exceptions;

public sealed class RequiredException : CoreBusinessException
{
    public RequiredException()
    {
    }

    public RequiredException(string msg) : base(msg)
    {
    }

    public RequiredException(string message, Exception inner) : base(message, inner)
    {
    }
}
