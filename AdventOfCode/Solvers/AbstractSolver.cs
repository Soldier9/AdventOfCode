using System.Text.RegularExpressions;

namespace AdventOfCode
{
    abstract class AbstractSolver
    {
        public readonly int Year;
        public readonly int Day;
        protected readonly string InputFile;
        public virtual bool PrioritizedSolver => false;

        protected AbstractSolver()
        {
            Year = Convert.ToInt32(Regex.Match(GetType().FullName!, @"(?<=AdventOfCode\.Solvers\.Year)\d+(?=\.Day)").Value);
            Day = Convert.ToInt32(Regex.Match(GetType().Name, @"(?<=Day)\d+(?=Solver)").Value);
            InputFile = Path.Combine(Directory.GetCurrentDirectory(), @"Inputs\" + Year + @"\Day" + Day + ".txt");
        }

        public abstract string Part1();
        public abstract string Part2();
    }
}
