namespace AdventOfCode.Solvers.Year2024
{
    class Day25Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<List<int>> locks = [];
            List<List<int>> keys = [];

            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingLock = false;
                bool parsingKey = false;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (!parsingKey && !parsingLock)
                    {
                        if (line == "#####")
                        {
                            parsingLock = true;
                            locks.Add([0, 0, 0, 0, 0]);
                        }
                        else
                        {
                            parsingKey = true;
                            keys.Add([-1, -1, -1, -1, -1]);
                        }
                        continue;
                    }
                    if (line.Length == 0) parsingKey = parsingLock = false;
                    if (parsingKey) for (int i = 0; i < line.Length; i++) keys[^1][i] += line[i] == '#' ? 1 : 0;
                    if (parsingLock) for (int i = 0; i < line.Length; i++) locks[^1][i] += line[i] == '#' ? 1 : 0;
                }
            }

            int result = 0;
            foreach (var l in locks) foreach (var k in keys)
                {
                    if (l.Zip(k).Where(lk => lk.First + lk.Second <= 5).Count() == 5) result++;
                }

            return result.ToString();
        }

        public override string Part2()
        {
            return "-".ToString();
        }
    }
}
