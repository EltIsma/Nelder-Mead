using NelderMeadLib.Models;

namespace NelderMeadLib.Interfaces;

public interface INelderMeadAlgorithm
{
    Task<Point> RunAsync(CancellationToken token);
    Task<Point> RunAsync(Simplex startTriangle, CancellationToken token);

    int GetSimplexSize();
    int GetArgumentsNumber();
    Simplex CreateSimplex(Point[] points);
}
