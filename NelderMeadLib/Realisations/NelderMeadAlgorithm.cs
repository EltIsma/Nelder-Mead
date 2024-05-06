using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NelderMeadLib.Exceptions;
using NelderMeadLib.Interfaces;

namespace NelderMeadLib.Models;

public class NelderMeadAlgorithm : INelderMeadAlgorithm
{
    ILogger? _logger;
    IFunction _function;
    AlgorithmParameters _parameters;

    public NelderMeadAlgorithm(
        IFunction function, 
        AlgorithmParameters parameters, 
        ILogger? logger)
    {
        if(function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        _function = function;
        _parameters = parameters;
        _logger = logger;
    }

    public async Task<Point> RunAsync(CancellationToken token)
    {
        return await Task.Run(() => Run(token), token);
    }
    public async Task<Point> RunAsync(Simplex? startSimplex, CancellationToken token)
    {
        return await Task.Run(() => Run(startSimplex, token), token);
    }

    Point Run(CancellationToken token)
    {
        var startTriangle = PrepareTriangle();
        return Run(startTriangle, token);
    }

    Point Run(Simplex? startSimplex, CancellationToken token)
    {
        var result = startSimplex;
        
        if(result is null)
        {
            result = PrepareTriangle();
        }

        int steps = 0;
        _logger?.LogStep(result, steps);
        steps++;

        while (!IsStopRequired(result, steps))
        {
            token.ThrowIfCancellationRequested();

            MakeStep(ref result);
            _logger?.LogStep(result, steps);
            steps++;
        }

        if (result is null)
        {
            throw new CantFindSolutionException();
        }

        _logger.LogSolution(result.Lowest);

        return result.Highest;
    }

    public Simplex CreateSimplex(Point[] points)
    {
        if(_function.GetArgumentsNumber() + 1 > points.Length)
        {
            throw new CantCreateSimplexException($"Not enough points to create simplex. " +
                $"Expecting {_function.GetArgumentsNumber} points.");
        }
        if(_function.GetArgumentsNumber() + 1 < points.Length)
        {
            throw new CantCreateSimplexException("Too much points to create simplex. " +
                $"Expecting {_function.GetArgumentsNumber} points.");
        }

        for(int i = 0; i < points.Length; i++)
        {
            points[i].Value = _function.Calculate(points[i].Coordinates);
        }

        return new Simplex(points);
    }

    public int GetSimplexSize()
    {
        return _function.GetArgumentsNumber() + 1;
    }
    public int GetArgumentsNumber() => _function.GetArgumentsNumber();

    Simplex PrepareTriangle()
    {
        int n = _function.GetArgumentsNumber();
        var points = new List<Point>(n + 1);
        for(int i = 0; i < n + 1; i++)
        {
            var coordinates = Enumerable.Range(0, n).Select(s => Random.Shared.Next(-1000, 1000)*Random.Shared.NextDouble()).ToArray();
            points.Add(new Point() { Coordinates = coordinates, Value = _function.Calculate(coordinates) });
        }

        return new Simplex(points.ToArray());
    }

    void MakeStep(ref Simplex from)
    {
        if(from is null)
        {
            return;
        }

        var center = GetMidOfSegment(from.Points);
        var reflected = ReflectPoint(from.Highest, center);
        if (from.Lowest.Value > reflected.Value)
        {
            var expended = ExpandPoint(center, reflected);
            if (expended.Value < reflected.Value)
            {
                var newPoints = from.Points
                    .Except(new[] { from.Highest })
                    .Union(new[] { expended });
                from = new Simplex(newPoints.ToArray());
                return;
            }
            if (expended.Value > reflected.Value)
            {
                var newPoints = from.Points
                    .Except(new[] { from.Highest })
                    .Union(new[] { reflected });
                from = new Simplex(newPoints.ToArray());
                return;
            }
        }
        if(from.Lowest.Value <= reflected.Value && reflected.Value < from.NextHighest.Value)
        {
            var newPoints = from.Points
                .Except(new[] { from.Highest })
                .Union(new[] { reflected });
            from = new Simplex(newPoints.ToArray());
            return;
        }
        if((from.NextHighest.Value <= reflected.Value && reflected.Value <= from.Highest.Value)
            || from.Highest.Value < reflected.Value)
        {
            if(from.NextHighest.Value <= reflected.Value && reflected.Value <= from.Highest.Value)
            {
                var newPoints = from.Points
                    .Except(new[] { from.Highest })
                    .Union(new[] { reflected });
                from = new Simplex(newPoints.ToArray());
            }
            var contractedPoint = ContractPoint(from.Highest, center);
            if(contractedPoint.Value < from.Highest.Value)
            {
                var newPoints = from.Points
                    .Except(new[] { from.Highest })
                    .Union(new[] { contractedPoint });
                from = new Simplex(newPoints.ToArray());
                return;
            }
            if(contractedPoint.Value > from.Highest.Value)
            {
                from = ShrinkTriangle(from, from.Lowest);
                return;
            }
        }

        return;
    }

    bool IsStopRequired(Simplex triangle, int steps)
    {
        double deviation = CalculateStandardDeviation(triangle.Points
            .Select(p => p.Value)
            .ToArray());

        return deviation < _parameters.SolutionPrecision
            && steps < _parameters.MaxIterations;
    }

    Point ReflectPoint(Point bestPoint, Point centerPoint)
    {
        double[] reflectedPoint = new double[bestPoint.Coordinates.Length];
        for (int i = 0; i < bestPoint.Coordinates.Length; i++)
        {
            reflectedPoint[i] = (1 + _parameters.ReflectionCoef) * centerPoint.Coordinates[i]
                - _parameters.ReflectionCoef * bestPoint.Coordinates[i];
        }

        return new Point() { Coordinates = reflectedPoint, Value = _function.Calculate(reflectedPoint) };
    }

    Point ExpandPoint(Point centerPoint, Point toStretch)
    {
        int n = centerPoint.Coordinates.Length;
        double[] stretchedPoint = new double[n];

        for (int i = 0; i < n; i++)
        {
            stretchedPoint[i] = (1 - _parameters.ExpansionCoef) * centerPoint.Coordinates[i]
                + _parameters.ExpansionCoef * toStretch.Coordinates[i];
        }

        return new Point() { Coordinates = stretchedPoint, Value = _function.Calculate(stretchedPoint) };
    }

    Point ContractPoint(Point bestPoint, Point centerPoint)
    {
        double[] contractedPoint = new double[bestPoint.Coordinates.Length];
        for (int i = 0; i < bestPoint.Coordinates.Length; i++)
        {
            contractedPoint[i] = (1 - _parameters.ContractionCoef) * centerPoint.Coordinates[i]
                + _parameters.ContractionCoef * bestPoint.Coordinates[i];
        }

        return new Point() { Coordinates = contractedPoint, Value = _function.Calculate(contractedPoint) };
    }

    Simplex ShrinkTriangle(Simplex triangle, Point worstPoint)
    {
        var points = triangle.Points.Except(new[] { worstPoint }).ToArray();
        var newPoints = new Point[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
        {
            newPoints[i] = ShrinkPoint(points[i], worstPoint);
        }
        newPoints[points.Length] = worstPoint;

        return new Simplex(newPoints);
    }

    Point ShrinkPoint(Point toShrink, Point worstPoint)
    {
        int n = worstPoint.Coordinates.Length;
        var shrinkedPoint = new double[n];
        for (int i = 0; i < n; i++)
        {
            shrinkedPoint[i] = worstPoint.Coordinates[i] +
                (toShrink.Coordinates[i] - worstPoint.Coordinates[i]) / 2;
        }

        return new Point() { Coordinates = shrinkedPoint, Value = _function.Calculate(shrinkedPoint) };
    }

    Point GetMidOfSegment(Point[] points)
    {
        var prepared = points.OrderByDescending(p => p.Value).Skip(1).ToArray();
        int n = prepared.First().Coordinates.Length;
        double[] mid = new double[n];
        foreach (var point in prepared)
        {
            for (int i = 0; i < point.Coordinates.Length; i++)
            {
                mid[i] += point.Coordinates[i];
            }
        }

        for (int i = 0; i < n; i++)
        {
            mid[i] /= n;
        }

        return new Point() { Coordinates = mid, Value = _function.Calculate(mid) };
    }

    double CalculateStandardDeviation(double[] values)
    {
        double avg = values.Average();
        double deviation = 0;
        foreach (var value in values)
        {
            deviation += Math.Pow(value - avg, 2);
        }

        deviation /= values.Length - 1;

        return Math.Sqrt(deviation);
    }
}
