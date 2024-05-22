namespace NelderMeadLib.Exceptions;

public class CantCreateSimplexException : Exception
{
    public CantCreateSimplexException() : base() { }
    public CantCreateSimplexException(string message) : base(message) { }
    public CantCreateSimplexException(string message, Exception innerException) : base(message, innerException) { }
}
