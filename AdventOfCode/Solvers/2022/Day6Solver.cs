namespace AdventOfCode.Solvers.Year2022
{
    class Day6Solver : AbstractSolver
    {
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    for (int i = 3; i < line.Length; i++)
                    {
                        bool valid = true;
                        for (int j = 3; j > 0; j--)
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                if (line[i - j] == line[i - k])
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) break;
                        }
                        if (valid) return (i + 1).ToString();
                    }
                }
            }

            return "".ToString();
        }

        public override string Part2()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    for (int i = 13; i < line.Length; i++)
                    {
                        bool valid = true;
                        for (int j = 13; j > 0; j--)
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                if (line[i - j] == line[i - k])
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) break;
                        }
                        if (valid) return (i + 1).ToString();
                    }
                }
            }

            return "".ToString();
        }
    }
}
