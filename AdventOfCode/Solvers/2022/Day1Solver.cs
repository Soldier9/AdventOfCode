namespace AdventOfCode.Solvers.Year2022
{
    class Day1Solver : AbstractSolver
    {
        List<int> elfs = new List<int>();
        public override string Part1()
        {
            elfs.Add(0);

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int cals = 0;
                    if (int.TryParse(line, out cals))
                    {
                        elfs[^1] += cals;
                    }
                    else
                    {
                        elfs.Add(0);
                    }
                }
            }

            return elfs.Max().ToString();

        }

        public override string Part2()
        {
            elfs.Sort();

            int result = 0;
            for (int i = elfs.Count - 1; i > elfs.Count - 4; i--)
            {
                result += elfs[i];
            }

            return result.ToString();
        }
    }
}
