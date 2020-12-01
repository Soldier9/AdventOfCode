using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day1Solver : AbstractSolver
    {
        private const int target = 2020;
        private HashSet<int> nums = new HashSet<int>();

        public override string Part1()
        {
            using(var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    nums.Add(int.Parse(input.ReadLine()));
                }
            }

            foreach(int x in nums)
            {
                if (nums.Contains(target - x)) return (x * (target - x)).ToString();
            }
            
            return "No solution found";
        }

        public override string Part2()
        {
            foreach (int x in nums)
            {
                foreach (int y in nums)
                {
                    if (x == y) continue;
                    if (nums.Contains(target - (x + y))) return (x * y * (target - (x + y))).ToString();
                }
            }

            return "No solution found";
        }
    }
}
