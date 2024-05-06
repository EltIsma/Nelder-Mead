using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;

namespace Console_NelderMead_App;

internal class ConsoleLogger : ILogger
{
    public void LogSolution(Point solution)
    {
        Console.WriteLine("------------------------");
        Console.WriteLine("Solution is");
        Console.WriteLine($"{solution.Value}, {FormatPoint(solution.Coordinates)}");
    }

    public void LogStep(Simplex message, int steps)
    {
        Console.WriteLine($"Step {steps}");
        Console.WriteLine($"Worst point: {message.Highest.Value}, {FormatPoint(message.Highest.Coordinates)}");
        Console.WriteLine($"Good point: {message.NextHighest.Value}, {FormatPoint(message.NextHighest.Coordinates)}");
        Console.WriteLine($"Best point: {message.Lowest.Value}, {FormatPoint(message.Lowest.Coordinates)}");
        Console.WriteLine();
    }

    private string FormatPoint(double[] coordinates)
    {
        return $"({String.Join("; ", coordinates)})";
    }
}
