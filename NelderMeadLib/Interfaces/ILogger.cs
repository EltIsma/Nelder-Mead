using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadLib.Models;

namespace NelderMeadLib.Interfaces;

public interface ILogger
{
    void LogStep(Simplex message, int step);
    void LogSolution(Point solution);
}
