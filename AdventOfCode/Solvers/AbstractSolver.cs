using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers;

internal abstract class AbstractSolver
{
    public readonly int Day;
    protected readonly string InputFile;
    public readonly int Year;

    protected AbstractSolver()
    {
        Year = Convert.ToInt32(Regex.Match(GetType().FullName!, @"(?<=AdventOfCode\.Solvers\.(Year|_))\d+(?=\.Day)").Value);
        Day = Convert.ToInt32(Regex.Match(GetType().Name, @"(?<=Day)\d+(?=Solver)").Value);
        InputFile = Path.Combine(Directory.GetCurrentDirectory(), "Inputs", Year.ToString(), "Day" + Day + ".txt");
    }

    public virtual bool PrioritizedSolver => false;
    public virtual bool HasVisualization => false;
    public virtual bool HasExtendedVisualization => false;

    public abstract string Part1();
    public abstract string Part2();
}