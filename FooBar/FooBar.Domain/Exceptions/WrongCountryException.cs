namespace FooBar.Domain.Exceptions;

public sealed class WrongCountryException : CoreBusinessException
{
    public WrongCountryException()
    {
    }
    public WrongCountryException(string msg) : base(msg)
    {
    }

    public WrongCountryException(string message, Exception inner) : base(message, inner)
    {
    }

}
