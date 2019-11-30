using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2019
{
    class Program
    {
        static IEnumerable<AbstractSolver> Solvers;

        static void Main(string[] args)
        {
            Solvers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == @"AdventOfCode2019.Solvers")
                .Select(s => (AbstractSolver)Activator.CreateInstance(s))
                .OrderBy(s => s.Day);

            var solver = Solvers.Last();
            Console.WriteLine("Solving Part 1");
            Console.WriteLine("Solution to Part 1: {0}", solver.Part1());

            Console.WriteLine("\r\nSolving Part 2");
            Console.WriteLine("Solution to Part 2: {0}", solver.Part2());

            Console.ReadLine();
        }
    }
}
