namespace AdventOfCode.Solvers.Year2024
{
    class Day4Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> Grid = new();
        int MaxX, MaxY;


        Dictionary<string, (int x, int y)> Directions = new Dictionary<string, (int x, int y)>
            {
                { "N", (0, -1) },
                { "NE", (1, -1) },
                { "E", (1, 0) },
                { "SE", (1, 1) },
                { "S", (0, 1) },
                { "SW", (-1, 1) },
                { "W", (-1, 0) },
                { "NW", (-1, -1) }
            };

        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    string line = input.ReadLine()!;
                    foreach (char c in line)
                    {
                        Grid.Add((x, y), c);
                        x++;
                    }
                    y++;
                    MaxX = x - 1;
                }
                MaxY = y - 1;
            }

            for (int x = 0; x <= MaxX; x++)
            {
                for (int y = 0; y <= MaxY; y++)
                {
                    foreach (string direction in Directions.Keys)
                    {
                        if (GetCharsAt((x, y), direction, 4) == "XMAS") result++;
                    }
                }
            }

            return result.ToString();
        }

        private (int x, int y) AddPos((int x, int y) pos1, (int x, int y) pos2)
        {
            return (pos1.x + pos2.x, pos1.y + pos2.y);
        }

        private string GetCharsAt((int x, int y) pos, string direction, int count)
        {
            if (count > 1)
            {
                (int x, int y) nextPos = AddPos(pos, Directions[direction]);
                if (!(nextPos.x < 0 || nextPos.x > MaxX || nextPos.y < 0 || nextPos.y > MaxY))
                {
                    return Grid[pos].ToString() + GetCharsAt(nextPos, direction, --count);
                }
            }
            return Grid[pos].ToString();
        }

        public override string Part2()
        {
            int result = 0;
            string[] variations = ["MAS", "SAM"];

            for (int x = 0; x <= MaxX; x++)
            {
                for (int y = 0; y <= MaxY; y++)
                {
                    if (!variations.Contains(GetCharsAt((x, y), "SE", 3))) continue;
                    if (!variations.Contains(GetCharsAt(AddPos((x, y), (2, 0)), "SW", 3))) continue;
                    result++;
                }
            }

            return result.ToString();
        }
    }
}
