namespace AdventOfCode.Solvers.Year2024
{
    class Day8Solver : AbstractSolver
    {
        Dictionary<char, List<(int x, int y)>> Antennas = [];
        (int x, int y) GridSize;

        public override string Part1()
        {
            HashSet<(int x, int y)> antinodes = [];

            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    string line = input.ReadLine()!;
                    foreach (char c in line)
                    {
                        if (c != '.')
                        {
                            Antennas.TryAdd(c, []);
                            Antennas[c].Add((x, y));
                        }
                        x++;
                    }
                    GridSize.x = x;
                    y++;
                }
                GridSize.y = y;
            }

            foreach (char c in Antennas.Keys)
            {
                for (int i = 0; i < Antennas[c].Count; i++)
                {
                    for (int j = 0; j < Antennas[c].Count; j++)
                    {
                        if (i == j) continue;
                        (int x, int y) diff = SubtractPos(Antennas[c][i], Antennas[c][j]);

                        (int x, int y) antiPos = AddPos(Antennas[c][i], diff);
                        if (InGrid(antiPos)) antinodes.Add(antiPos);

                        antiPos = SubtractPos(Antennas[c][j], diff);
                        if (InGrid(antiPos)) antinodes.Add(antiPos);
                    }
                }
            }
            return antinodes.Count.ToString();
        }

        private bool InGrid((int x, int y) pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= GridSize.x || pos.y >= GridSize.y) return false;
            return true;
        }

        private (int x, int y) SubtractPos((int x, int y) a, (int x, int y) b)
        {
            return (a.x - b.x, a.y - b.y);
        }

        private (int x, int y) AddPos((int x, int y) a, (int x, int y) b)
        {
            return (a.x + b.x, a.y + b.y);
        }

        public override string Part2()
        {
            HashSet<(int x, int y)> antinodes = [];
            foreach (char c in Antennas.Keys)
            {
                for (int i = 0; i < Antennas[c].Count; i++)
                {
                    for (int j = 0; j < Antennas[c].Count; j++)
                    {
                        if (i == j) continue;
                        (int x, int y) diff = SubtractPos(Antennas[c][i], Antennas[c][j]);


                        antinodes.Add(Antennas[c][i]);
                        (int x, int y) antiPos = AddPos(Antennas[c][i], diff);
                        while (InGrid(antiPos))
                        {
                            antinodes.Add(antiPos);
                            antiPos = AddPos(antiPos, diff);
                        }

                        antiPos = SubtractPos(Antennas[c][i], diff);
                        while (InGrid(antiPos))
                        {
                            antinodes.Add(antiPos);
                            antiPos = SubtractPos(antiPos, diff);
                        }
                    }
                }
            }
            return antinodes.Count.ToString();
        }
    }
}
