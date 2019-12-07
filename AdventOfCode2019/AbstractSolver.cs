using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2019
{
    abstract class AbstractSolver
    {
        public readonly int Day;
        protected readonly string InputFile;
        public virtual bool PrioritizedSolver => false;
        protected AbstractSolver()
        {
            Day = Convert.ToInt32(Regex.Match(GetType().Name, @"(?<=Day)\d+(?=Solver)").Value);
            InputFile = Path.Combine(Directory.GetCurrentDirectory(), @"Inputs\Day" + Day + ".txt");
        }

        

        public abstract string Part1();
        public abstract string Part2();
    }
}
