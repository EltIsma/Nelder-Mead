using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;
using NelderMeadLib.Realisations;

namespace Console_NelderMead_App;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        IFunction function = new Function("f(x,y)=(x^2+y-11)^2+(x+y^2-7)^2");
        INelderMeadAlgorithm alg = new NelderMeadAlgorithm(function, new AlgorithmParameters(), new ConsoleLogger());
        var points = new Point[]
        {
            new Point(){Coordinates = new double[]{ 5, 5 } },
            new Point(){Coordinates = new double[]{ -4, 3} },
            new Point(){Coordinates = new double[]{ -7, 7 } }
        };

        var result = alg.RunAsync(alg.CreateSimplex(points), default).Result;
    }
}
