namespace NelderMeadLib.Models;

public record AlgorithmParameters
{
    public int MaxIterations { get; set; } = 10000;
    public double ReflectionCoef { get; set; } = 1.0;
    public double ContractionCoef { get; set; } = 0.5;
    public double ShrinkCoef { get; set; } = 0.5;
    public double ExpansionCoef { get; set; } = 2.0;
    public double SolutionPrecision { get; set; } = 1e-8;

    public bool UseUserSimplex { get; set; } = false;
}
