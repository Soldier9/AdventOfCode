namespace AdventOfCode.Solvers.Year2021
{
    class Day6Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int> fish = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                fish = new List<int>(input.ReadLine()!.Split(',').Select(f => int.Parse(f)));
            }

            for (int i = 0; i < 80; i++)
            {
                int count = fish.Count;
                for (int n = 0; n < count; n++)
                {
                    fish[n]--;
                    if (fish[n] < 0)
                    {
                        fish[n] = 6;
                        fish.Add(8);
                    }
                }
            }

            return fish.Count.ToString();
        }

        public override string Part2()
        {
            List<long[]> fish = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                List<int> tmpFish = new(input.ReadLine()!.Split(',').Select(f => int.Parse(f)));
                for (int n = 0; n < 9; n++)
                {
                    fish.Add(new long[2] { n, 0 });
                }

                foreach (int f in tmpFish)
                {
                    fish[f][1]++;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                int group0Index = 0;
                int group6Index = 0;

                for (int groupIdx = 0; groupIdx < fish.Count; groupIdx++)
                {
                    if (fish[groupIdx][0] == 0) group0Index = groupIdx;
                    fish[groupIdx][0]--;
                    if (fish[groupIdx][0] == 6) group6Index = groupIdx;
                }

                fish[group6Index][1] += fish[group0Index][1];
                fish[group0Index][0] = 8;
            }

            long result = 0;
            foreach (long[] group in fish) result += group[1];

            return result.ToString();
        }
    }
}
