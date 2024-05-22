using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;
using System.Collections.Generic;

namespace Nelder_Mead_App;

internal class CustomLogger : ILogger
{
    public Point Solution { get; private set; }

    private List<Simplex> steps;
    public IReadOnlyCollection<Simplex> Steps => steps;

    public CustomLogger()
    {
        steps = new List<Simplex>();
    }

    public void LogSolution(Point solution)
    {
        Solution = solution;
    }

    public void LogStep(Simplex message, int step)
    {
        steps.Add(message);
    }

    public void Clear()
    {
        steps.Clear();
    }
}
