using NelderMeadLib.Exceptions;
using NelderMeadLib.Interfaces;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mxparser = org.mariuszgromada.math.mxparser;

namespace NelderMeadLib.Realisations;

public class Function : IFunction
{
    Mxparser.Function function;

    public Function(string expression)
    {
        License.iConfirmNonCommercialUse("sidelnikoff_a");
        
        function = new Mxparser.Function(expression);

        if(!function.checkSyntax())
        {
            throw new CantParseExpressionException(function.getErrorMessage());
        }
    }

    public double Calculate(double[] coordinates) => function.calculate(coordinates);
    public int GetArgumentsNumber() => function.getArgumentsNumber();
}
