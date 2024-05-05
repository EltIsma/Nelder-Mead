using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadLib.Exceptions;

public class CantParseExpressionException : Exception
{
    public CantParseExpressionException() : base() { }
    public CantParseExpressionException(string message) : base(message) { }
    public CantParseExpressionException(string message, Exception innerException) : base(message, innerException) { }
}
