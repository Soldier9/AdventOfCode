using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day9Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int[]> grid = new List<int[]>();

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    grid.Add(input.ReadLine().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray());
                }
            }

            int result = 0;
            int height = grid.Count;
            int width = grid[0].Length;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > 0 && grid[y][x - 1] <= grid[y][x]) continue;
                    if (x < width - 1 && grid[y][x + 1] <= grid[y][x]) continue;

                    if (y > 0 && grid[y - 1][x] <= grid[y][x]) continue;
                    if (y < height - 1 && grid[y + 1][x] <= grid[y][x]) continue;
                    result += (1 + grid[y][x]);
                }
            }

            return result.ToString();
        }


        public override string Part2()
        {
            List<int[]> grid = new List<int[]>();

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    grid.Add(input.ReadLine().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray());
                }
            }

            int result = 1;
            int height = grid.Count;
            int width = grid[0].Length;
            List<(int, int)> lowPoints = new List<(int, int)>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > 0 && grid[y][x - 1] <= grid[y][x]) continue;
                    if (x < width - 1 && grid[y][x + 1] <= grid[y][x]) continue;

                    if (y > 0 && grid[y - 1][x] <= grid[y][x]) continue;
                    if (y < height - 1 && grid[y + 1][x] <= grid[y][x]) continue;

                    lowPoints.Add((x, y));
                }
            }

            List<HashSet<(int, int)>> basinGrids = new List<HashSet<(int, int)>>();
            foreach ((int, int) point in lowPoints)
            {
                HashSet<(int, int)> basinGrid = new HashSet<(int, int)>();
                basinGrids.Add(basinGrid);
                basinGrid.Add(point);
                AddNeighbors(grid, point, basinGrid);
            }

            basinGrids = basinGrids.OrderByDescending(bg => bg.Count).ToList();
            for (int i = 0; i < 3; i++)
            {
                result *= basinGrids[i].Count;
            }

            return result.ToString();
        }

        public void AddNeighbors(List<int[]> grid, (int x, int y) point, HashSet<(int, int)> basinGrid)
        {
            List<(int x, int y)> neighbors = new List<(int, int)>();

            if (point.x > 0) neighbors.Add((point.x - 1, point.y));
            if (point.x < grid[0].Length - 1) neighbors.Add((point.x + 1, point.y));

            if (point.y > 0) neighbors.Add((point.x, point.y - 1));
            if (point.y < grid.Count - 1) neighbors.Add((point.x, point.y + 1));

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!basinGrid.Contains(neighbors[i]) && grid[neighbors[i].y][neighbors[i].x] < 9)
                {
                    basinGrid.Add(neighbors[i]);
                    AddNeighbors(grid, neighbors[i], basinGrid);
                }
            }
        }
    }
}
