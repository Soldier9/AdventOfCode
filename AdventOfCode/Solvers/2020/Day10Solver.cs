using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day10Solver : AbstractSolver
    {
        List<int> adapters = new List<int>();
        int finalJoltage = 0;
        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    adapters.Add(int.Parse(input.ReadLine()));
                }
            }

            int dif1 = 0;
            int dif3 = 0;
            adapters.Sort();
            foreach (var adapter in adapters)
            {
                switch (adapter - finalJoltage)
                {
                    case 1: dif1++; break;
                    case 3: dif3++; break;
                }
                finalJoltage = adapter;
            }
            dif3++;
            finalJoltage += 3;

            return (dif1 * dif3).ToString();
        }

        public override string Part2()
        {
            long paths = 0;
            for (int i = 0; i < adapters.Count; i++)
            {
                if (adapters[i] > 3) break;
                paths += validPathsToEnd(adapters[i], i);
            }

            return paths.ToString();
        }

        Dictionary<int, long> foundPaths = new Dictionary<int, long>();
        public long validPathsToEnd(int adapter, int index)
        {
            if (finalJoltage - adapter <= 3) return 1;
            if (foundPaths.ContainsKey(adapter)) return foundPaths[adapter];

            long paths = 0;
            for (int i = index + 1; i < adapters.Count; i++)
            {
                if (adapters[i] - adapter > 3) break;
                paths += validPathsToEnd(adapters[i], i);
            }
            foundPaths.Add(adapter, paths);
            return paths;
        }
    }
}
