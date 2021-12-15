namespace AdventOfCode.Solvers.Year2019
{
    class Day1Solver : AbstractSolver
    {
        public override string Part1()
        {
            long totalFuelRequired = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    int mass = int.Parse(input.ReadLine()!);
                    int fuelRequired = (int)Math.Floor(mass / 3.0) - 2;
                    totalFuelRequired += fuelRequired;
                }
            }

            return totalFuelRequired.ToString();
        }

        public override string Part2()
        {
            long totalFuelRequired = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    int mass = int.Parse(input.ReadLine()!);
                    int fuelRequired = GetFuelRequired(mass);
                    totalFuelRequired += fuelRequired;
                }
            }

            return totalFuelRequired.ToString();
        }

        int GetFuelRequired(int mass)
        {
            int fuel = Math.Max((int)Math.Floor(mass / 3.0) - 2, 0);
            if (fuel > 0) fuel += (GetFuelRequired(fuel));
            return fuel;
        }
    }
}
