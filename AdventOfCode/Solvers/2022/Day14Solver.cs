﻿namespace AdventOfCode.Solvers.Year2022
{
    class Day14Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> map = new();
        int bottom = 0;

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    List<(int x, int y)> points = input.ReadLine()!.Split(" -> ").Select(c => { string[] p = c.Split(","); return (int.Parse(p[0]!), int.Parse(p[1])); }).ToList<(int x, int y)>();
                    for (int i = 1; i < points.Count; i++)
                    {
                        (int x, int y) from = points[i - 1];
                        (int x, int y) to = points[i];
                        if (from.x < to.x)
                        {
                            for (int x = from.x; x <= to.x; x++) if (!map.ContainsKey((x, to.y))) map.Add((x, to.y), '#');
                        }
                        else if (from.x > to.x)
                        {
                            for (int x = from.x; x >= to.x; x--) if (!map.ContainsKey((x, to.y))) map.Add((x, to.y), '#');
                        }
                        else if (from.y < to.y)
                        {
                            for (int y = from.y; y <= to.y; y++) if (!map.ContainsKey((to.x, y))) map.Add((to.x, y), '#');
                        }
                        else
                        {
                            for (int y = from.y; y >= to.y; y--) if (!map.ContainsKey((to.x, y))) map.Add((to.x, y), '#');
                        }
                        bottom = Math.Max(Math.Max(bottom, from.y), to.y);
                    }
                }
            }

            int result = 0;
            while (FlowSand((500, 0))) result++;
            return result.ToString();
        }

        class DoneException : Exception { }

        public bool FlowSand((int x, int y) sand, bool part2 = false)
        {
            if (!part2 && sand.y > bottom) return false;
            else if (part2 && sand.y == bottom + 1)
            {
                map.Add((sand.x, sand.y), 'o');
                return true;
            }

            if (!map.ContainsKey((sand.x, sand.y + 1))) return FlowSand((sand.x, sand.y + 1), part2);
            else if (!map.ContainsKey((sand.x - 1, sand.y + 1))) return FlowSand((sand.x - 1, sand.y + 1), part2);
            else if (!map.ContainsKey((sand.x + 1, sand.y + 1))) return FlowSand((sand.x + 1, sand.y + 1), part2);

            map.Add((sand.x, sand.y), 'o');
            if (part2 && sand == (500, 0)) return false;
            return true;
        }

        public override string Part2()
        {
            foreach ((int x, int y) sand in map.Where(p => p.Value == 'o').Select(p => p.Key)) map.Remove(sand);

            int result = 1;
            while (FlowSand((500, 0), true)) result++;
            return result.ToString();
        }
    }
}
