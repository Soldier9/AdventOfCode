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
                .Where(t => t.BaseType.Name == "AbstractSolver")
                .Select(s => (AbstractSolver)Activator.CreateInstance(s))
                .OrderBy(s => s.Day);

            var solver = Solvers.Last();
            Console.WriteLine("Advent of Code Day {0}", solver.Day);
            Console.WriteLine("\r\nSolving Part 1");
            try
            {
                Console.WriteLine("Solution to Part 1: {0}", solver.Part1());
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Solution not implemented");
            }

            Console.WriteLine("\r\nSolving Part 2");
            try
            {
                Console.WriteLine("Solution to Part 2: {0}", solver.Part2());
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Solution not implemented");
            }

            Console.ReadLine();
        }
    }
}
