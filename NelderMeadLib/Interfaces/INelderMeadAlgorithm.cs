using NelderMeadLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NelderMeadLib.Interfaces;

public interface INelderMeadAlgorithm
{
    Task<Point> RunAsync(CancellationToken token);
    Task<Point> RunAsync(Simplex startTriangle, CancellationToken token);

    int GetSimplexSize();
    int GetArgumentsNumber();
    Simplex CreateSimplex(Point[] points);
}
