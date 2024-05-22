using NelderMeadLib.Models;

namespace NelderMeadLib.Interfaces;

public interface ILogger
{
    void LogStep(Simplex message, int step);
    void LogSolution(Point solution);
}
