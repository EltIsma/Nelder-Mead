namespace NelderMeadLib.Models;

public record Point
{
    public double Value { get; set; }
    public required double[] Coordinates { get; init; }
}
