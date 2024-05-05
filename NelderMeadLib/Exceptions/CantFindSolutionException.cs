using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadLib.Exceptions;

public class CantFindSolutionException : Exception
{
    public CantFindSolutionException() : base() { }
    public CantFindSolutionException(string message) : base(message) { }
    public CantFindSolutionException(string message, Exception innerException) : base(message, innerException) { }
}
