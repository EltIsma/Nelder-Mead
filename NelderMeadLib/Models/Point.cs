using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadLib.Models;

public record Point
{
    public double Value { get; set; }
    public required double[] Coordinates { get; init; }
}
