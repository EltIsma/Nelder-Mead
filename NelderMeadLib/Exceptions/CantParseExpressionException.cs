namespace NelderMeadLib.Exceptions;

public class CantParseExpressionException : Exception
{
    public CantParseExpressionException() : base() { }
    public CantParseExpressionException(string message) : base(message) { }
    public CantParseExpressionException(string message, Exception innerException) : base(message, innerException) { }
}
