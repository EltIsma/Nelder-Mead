using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadLib.Exceptions;

public class CantCreateSimplexException : Exception
{
    public CantCreateSimplexException() : base() { }
    public CantCreateSimplexException(string message) : base(message) { }
    public CantCreateSimplexException(string message, Exception innerException) : base(message, innerException) { }
}
