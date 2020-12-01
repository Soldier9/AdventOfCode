using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day1Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int> nums = new List<int>();
            
            using(var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    nums.Add(int.Parse(input.ReadLine()));
                }
            }

            foreach(int x in nums)
            {
                foreach(int y in nums)
                {
                    if (x + y == 2020) return (x * y).ToString();
                }
            }
            
            return "No solution found";
        }

        public override string Part2()
        {
            List<int> nums = new List<int>();

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    nums.Add(int.Parse(input.ReadLine()));
                }
            }

            foreach (int x in nums)
            {
                foreach (int y in nums)
                {
                    if (x == y) continue;
                    foreach (int z in nums)
                    {
                        if (x + y + z == 2020) return (x * y * z).ToString();
                    }
                }
            }

            return "No solution found";
        }
    }
}
