namespace AdventOfCode.Solvers.Year2023
{
    class Day3Solver : AbstractSolver
    {
        readonly List<string> schematic = new();
        int height = 0;
        int width = 0;

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    schematic.Add(line);
                }
            }

            height = schematic.Count;
            width = schematic[0].Length;

            List<int> numbers = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (int.TryParse(schematic[y][x].ToString(), out _) && NextToSymbol(x, y))
                    {
                        numbers.Add(GetNumber(x, y));
                        while (x <= width - 1 && int.TryParse(schematic[y][x].ToString(), out _)) x++;
                    }
                }
            }

            return numbers.Sum().ToString();
        }

        private int GetNumber(int x, int y)
        {
            int startX = x;
            int endX = x;
            while (startX >= 0 && int.TryParse(schematic[y][startX].ToString(), out _)) startX--;
            startX++;
            while (endX <= width - 1 && int.TryParse(schematic[y][endX].ToString(), out _)) endX++;
            endX--;

            return int.Parse(schematic[y].Substring(startX, endX - startX + 1));
        }


        private bool NextToSymbol(int x, int y)
        {
            for (int i = Math.Max(0, y - 1); i <= Math.Min(height - 1, y + 1); i++)
            {
                for (int j = Math.Max(0, x - 1); j <= Math.Min(width - 1, x + 1); j++)
                {
                    if (schematic[i][j] != '.' && !int.TryParse(schematic[i][j].ToString(), out _)) return true;
                }
            }

            return false;
        }


        public override string Part2()
        {
            Dictionary<(int x, int y), List<int>> gearRatios = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (int.TryParse(schematic[y][x].ToString(), out _) && AdjacentGear(x, y) != (-1, -1))
                    {
                        (int gearX, int gearY) gearPos = AdjacentGear(x, y);

                        if (!gearRatios.ContainsKey(gearPos)) gearRatios.Add(AdjacentGear(x, y), new());
                        gearRatios[gearPos].Add(GetNumber(x, y));

                        while (x <= width - 1 && int.TryParse(schematic[y][x].ToString(), out _)) x++;
                    }
                }
            }

            int result = 0;
            foreach (KeyValuePair<(int x, int y), List<int>> gear in gearRatios.Where(gr => gr.Value.Count == 2)) result += gear.Value[0] * gear.Value[1];

            return result.ToString();
        }


        private (int x, int y) AdjacentGear(int x, int y)
        {
            for (int i = Math.Max(0, y - 1); i <= Math.Min(height - 1, y + 1); i++)
            {
                for (int j = Math.Max(0, x - 1); j <= Math.Min(width - 1, x + 1); j++)
                {
                    if (schematic[i][j] == '*') return (j, i);
                }
            }

            return (-1, -1);
        }
    }
}
