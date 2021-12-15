namespace AdventOfCode.Solvers.Year2020
{
    class Day5Solver : AbstractSolver
    {
        readonly List<int> seatIDs = new();
        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int rowMin = -1;
                    int rowMax = 127;
                    int colMin = -1;
                    int colMax = 7;

                    foreach (char c in line)
                    {
                        switch (c)
                        {
                            case 'F': rowMax -= (rowMax - rowMin) / 2; break;
                            case 'B': rowMin += (rowMax - rowMin) / 2; break;
                            case 'L': colMax -= (colMax - colMin) / 2; break;
                            case 'R': colMin += (colMax - colMin) / 2; break;
                        }
                    }

                    seatIDs.Add((rowMax * 8) + colMax);
                }
            }
            seatIDs.Sort();

            return seatIDs[^1].ToString();
        }

        public override string Part2()
        {
            HashSet<int> seatIDs = new(this.seatIDs);
            for (int i = 0; i < this.seatIDs[^1]; i++)
            {
                if (seatIDs.Contains(i)) continue;
                if (!seatIDs.Contains(i - 1)) continue;
                if (!seatIDs.Contains(i + 1)) continue;
                return i.ToString();
            }

            return "No solution found";
        }
    }
}
