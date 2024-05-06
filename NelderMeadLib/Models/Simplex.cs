namespace NelderMeadLib.Models;

public class Simplex
{
    private Point[] _points;

    public Simplex(Point[] points)
    {
        _points = (Point[])points.Clone();
    }

    public Point[] Points => _points.ToArray();

    public Point Highest => _points.OrderByDescending(p => p.Value).First();
    public Point NextHighest => _points.OrderByDescending(p => p.Value).Skip(1).First();
    public Point Lowest => _points.OrderByDescending(p => p.Value).Last();
}
