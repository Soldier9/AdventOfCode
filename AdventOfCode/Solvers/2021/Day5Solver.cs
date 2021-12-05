using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2021
{
    class Day5Solver : AbstractSolver
    {
        class Grid : Dictionary<int, Dictionary<int, int>>
        {
            public void AddLine((int x, int y) start, (int x, int y) end)
            {
                var xDirection = (end.x - start.x);
                var yDirection = (end.y - start.y);

                if (xDirection != 0) xDirection /= Math.Abs(xDirection);
                else xDirection = 1;
                if (yDirection != 0) yDirection /= Math.Abs(yDirection);
                else yDirection = 1;

                bool diagonal = (start.x != end.x && start.y != end.y);

                for (int y = start.y; yDirection < 0 ? y >= end.y : y <= end.y; y += yDirection)
                {
                    for (int x = start.x; xDirection < 0 ? x >= end.x : x <= end.x; x += xDirection)
                    {
                        if (!this.ContainsKey(y)) this.Add(y, new Dictionary<int, int>());
                        if (!this[y].ContainsKey(x)) this[y].Add(x, 0);
                        this[y][x]++;

                        if (diagonal) y += yDirection;
                    }
                }
            }
        }

        public override string Part1()
        {
            Grid grid = new Grid();

            using (var input = File.OpenText(InputFile))
            {
                Regex parser = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)");
                while (!input.EndOfStream)
                {
                    Match points = parser.Match(input.ReadLine());
                    if (int.Parse(points.Groups[1].Value) == int.Parse(points.Groups[3].Value) || int.Parse(points.Groups[2].Value) == int.Parse(points.Groups[4].Value))
                    {
                        grid.AddLine((int.Parse(points.Groups[1].Value), int.Parse(points.Groups[2].Value)), (int.Parse(points.Groups[3].Value), int.Parse(points.Groups[4].Value)));
                    }
                }
            }

            int result = 0;
            foreach (var line in grid)
            {
                foreach (var point in line.Value)
                {
                    if (point.Value > 1) result++;
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            Grid grid = new Grid();

            using (var input = File.OpenText(InputFile))
            {
                Regex parser = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)");
                while (!input.EndOfStream)
                {
                    Match points = parser.Match(input.ReadLine());
                    grid.AddLine((int.Parse(points.Groups[1].Value), int.Parse(points.Groups[2].Value)), (int.Parse(points.Groups[3].Value), int.Parse(points.Groups[4].Value)));
                }
            }

            int result = 0;
            foreach (var line in grid)
            {
                foreach (var point in line.Value)
                {
                    if (point.Value > 1) result++;
                }
            }

            return result.ToString();
        }
    }
}
