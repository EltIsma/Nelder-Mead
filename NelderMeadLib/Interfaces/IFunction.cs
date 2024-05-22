namespace NelderMeadLib.Interfaces;

public interface IFunction
{
    double Calculate(double[] coordinates);
    int GetArgumentsNumber();
}
