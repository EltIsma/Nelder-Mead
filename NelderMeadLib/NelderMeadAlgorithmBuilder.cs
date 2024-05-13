using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;
using NelderMeadLib.Realisations;
using Function = NelderMeadLib.Realisations.Function;

namespace NelderMeadLib;

public class NelderMeadAlgorithmBuilder
{
    private AlgorithmParameters _parameters = new AlgorithmParameters();
    private IFunction? _function;
    private ILogger? _logger;

    public NelderMeadAlgorithmBuilder SetMaxIterations(int max)
    {
        if (max <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }
        _parameters.MaxIterations = max;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetReflectionCoef(double coef)
    {
        _parameters.ReflectionCoef = coef;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetContractionCoef(double coef)
    {
        _parameters.ContractionCoef = coef;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetShrinkCoef(double coef)
    {
        _parameters.ShrinkCoef = coef;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetExpansionCoef(double coef)
    {
        _parameters.ExpansionCoef = coef;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetSolutionPrecision(double precision)
    {
        _parameters.SolutionPrecision = precision;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetFunction(string function)
    {
        _function = new Function(function);
        return this;
    }

    public NelderMeadAlgorithmBuilder SetFunction(Function function)
    {
        _function = function;
        return this;
    }

    public NelderMeadAlgorithmBuilder SetLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    public NelderMeadAlgorithm Build()
    {
        return new NelderMeadAlgorithm(_function, _parameters, _logger);
    }
}
