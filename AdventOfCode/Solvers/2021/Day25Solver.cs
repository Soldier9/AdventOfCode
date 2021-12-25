using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day25Solver : AbstractSolver
    {
        public override string Part1()
        {
            Dictionary<(int x, int y), char> map = new();
            int width = 0;
            int height = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;

                    int x = 0;
                    foreach (char c in line)
                    {
                        map.Add((x, y), c);
                        x++;
                    }
                    width = x;
                    y++;
                }
                height = y;
            }

            for (int n = 1; n > 0; n++)
            {
                if (Program.VisualizationEnabled || true) PrintMap(map, width, height);
                Dictionary<(int x, int y), char> newMap = new();
                bool thingsMoved = false;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        switch (map[(x, y)])
                        {
                            case '>':
                                int nextX = (x == width - 1) ? 0 : x + 1;
                                if (map[(nextX, y)] == '.')
                                {
                                    if (newMap.ContainsKey((nextX, y)) && newMap[(nextX, y)] == '.') newMap[(nextX, y)] = map[(x, y)];
                                    else newMap.Add((nextX, y), map[(x, y)]);
                                    newMap.Add((x, y), '.');
                                    thingsMoved = true;
                                }
                                else
                                {
                                    newMap.Add((x, y), map[(x, y)]);
                                }
                                break;
                            default:
                                _ = newMap.TryAdd((x, y), map[(x, y)]);
                                break;
                        }
                    }
                }

                map = newMap;
                if (Program.VisualizationEnabled) PrintMap(map, width, height);
                newMap = new();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        switch (map[(x, y)])
                        {
                            case 'v':
                                int nextY = (y == height - 1) ? 0 : y + 1;
                                if (map[(x, nextY)] == '.')
                                {
                                    if (newMap.ContainsKey((x, nextY)) && newMap[(x, nextY)] == '.') newMap[(x, nextY)] = map[(x, y)];
                                    else newMap.Add((x, nextY), map[(x, y)]);
                                    newMap.Add((x, y), '.');
                                    thingsMoved = true;
                                }
                                else
                                {
                                    newMap.Add((x, y), map[(x, y)]);
                                }
                                break;

                            default:
                                _ = newMap.TryAdd((x, y), map[(x, y)]);
                                break;
                        }
                    }
                }

                if (thingsMoved)
                {
                    map = newMap;
                }
                else
                {
                    return n.ToString();
                }
            }

            throw new NotImplementedException();
        }

        static void PrintMap(Dictionary<(int x, int y), char> map, int width, int height)
        {
            StringBuilder sb = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _ = sb.Append(map[(x, y)]);
                }
                _ = sb.Append("\r\n");
            }
            Program.PrintData(sb.ToString(), 10);
        }

        public override string Part2()
        {
            return "Solve manually!";
        }
    }
}
