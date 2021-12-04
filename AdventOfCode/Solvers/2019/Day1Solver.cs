using System;
using System.IO;

namespace AdventOfCode.Solvers.Year2019
{
    class Day1Solver : AbstractSolver
    {
        public override string Part1()
        {
            long totalFuelRequired = 0;
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    int mass = Int32.Parse(input.ReadLine());
                    int fuelRequired = (int)Math.Floor(mass / 3.0) - 2;
                    totalFuelRequired += fuelRequired;
                }
            }

            return totalFuelRequired.ToString();
        }

        public override string Part2()
        {
            long totalFuelRequired = 0;
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    int mass = Int32.Parse(input.ReadLine());
                    int fuelRequired = getFuelRequired(mass);
                    totalFuelRequired += fuelRequired;
                }
            }

            return totalFuelRequired.ToString();
        }

        int getFuelRequired(int mass)
        {
            int fuel = Math.Max((int)Math.Floor(mass / 3.0) - 2, 0);
            if (fuel > 0) fuel += (getFuelRequired(fuel));
            return fuel;
        }
    }
}
