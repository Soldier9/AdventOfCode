using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2019
{
    class Day24Solver : AbstractSolver
    {
        class GridComparer : IEqualityComparer<List<char[]>>
        {
            public bool Equals(List<char[]> x, List<char[]> y)
            {
                if (x.Count != y.Count) return false;
                for (var i = 0; i < x.Count; i++)
                {
                    if (!x[i].SequenceEqual(y[i])) return false;
                }
                return true;
            }

            public int GetHashCode(List<char[]> obj)
            {
                int hash = -346745677;
                for (var i = 0; i < obj.Count; i++)
                {
                    for (int j = 0; j < obj[i].Length; j++)
                    {
                        hash += i * 63456 * j * 234 * obj[i][j].GetHashCode();
                    }
                }
                return hash;
            }
        }

        public override string Part1()
        {
            var grid = new List<char[]>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    grid.Add(input.ReadLine().ToCharArray());
                }
            }

            int width = grid[0].Length;
            int height = grid.Count;
            var seenGrids = new HashSet<List<char[]>>(new GridComparer());

            while (!seenGrids.Contains(grid))
            {
                seenGrids.Add(grid);
                var newGrid = new List<char[]>();
                foreach (var line in grid)
                {
                    var newLine = new char[line.Length];
                    line.CopyTo(newLine, 0);
                    newGrid.Add(newLine);
                }

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        int neighborBugs = 0;
                        if (x > 0 && grid[y][x - 1] == '#') neighborBugs++;
                        if (x < width - 1 && grid[y][x + 1] == '#') neighborBugs++;
                        if (y > 0 && grid[y - 1][x] == '#') neighborBugs++;
                        if (y < height - 1 && grid[y + 1][x] == '#') neighborBugs++;

                        if (grid[y][x] == '#' && neighborBugs == 1) newGrid[y][x] = '#';
                        else if (grid[y][x] == '.' && (neighborBugs == 1 || neighborBugs == 2)) newGrid[y][x] = '#';
                        else newGrid[y][x] = '.';
                    }
                }
                grid = newGrid;
            }

            int result = 0;
            int pow = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (grid[y][x] == '#') result += (int)Math.Pow(2, pow);
                    pow++;
                }
            }

            return result.ToString();
        }

        List<char[]> CreateEmptyGrid()
        {
            var newGrid = new List<char[]>();
            newGrid.Add(".....".ToCharArray());
            newGrid.Add(".....".ToCharArray());
            newGrid.Add("..?..".ToCharArray());
            newGrid.Add(".....".ToCharArray());
            newGrid.Add(".....".ToCharArray());
            return newGrid;
        }
        public override string Part2()
        {

            var grids = new Dictionary<int, List<char[]>>();
            using (var input = File.OpenText(InputFile))
            {
                var grid = new List<char[]>();
                while (!input.EndOfStream)
                {
                    grid.Add(input.ReadLine().ToCharArray());
                }
                grid[2][2] = '?';
                grids.Add(0, grid);
            }

            int width = grids[0][0].Length;
            int height = grids[0].Count;

            for (var n = 0; n < 200; n++)
            {

                var lowestGrid = grids.Keys.Min();
                var highestGrid = grids.Keys.Max();
                bool needLowerGrid = false;
                bool needHigherGrid = false;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (grids[lowestGrid][y][x] == '#') needLowerGrid = true;
                        if (grids[highestGrid][y][x] == '#') needHigherGrid = true;
                        if (needLowerGrid && needHigherGrid) break;
                    }
                    if (needLowerGrid && needHigherGrid) break;
                }
                if (needLowerGrid) grids.Add(lowestGrid - 1, CreateEmptyGrid());
                if (needHigherGrid) grids.Add(highestGrid + 1, CreateEmptyGrid());


                var newGrids = new Dictionary<int, List<char[]>>();
                foreach (var level in grids.Keys)
                {
                    var newGrid = CreateEmptyGrid();
                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            if (x == 2 && y == 2) continue;

                            int neighborBugs = 0;
                            if (x > 0 && grids[level][y][x - 1] == '#') neighborBugs++;
                            if (x < width - 1 && grids[level][y][x + 1] == '#') neighborBugs++;
                            if (y > 0 && grids[level][y - 1][x] == '#') neighborBugs++;
                            if (y < height - 1 && grids[level][y + 1][x] == '#') neighborBugs++;

                            if (grids.ContainsKey(level - 1))
                            {
                                if (x == 0 && grids[level - 1][2][1] == '#') neighborBugs++;
                                if (x == width - 1 && grids[level - 1][2][3] == '#') neighborBugs++;
                                if (y == 0 && grids[level - 1][1][2] == '#') neighborBugs++;
                                if (y == height - 1 && grids[level - 1][3][2] == '#') neighborBugs++;
                            }

                            if (grids.ContainsKey(level + 1))
                            {
                                if (x == 2 && y == 1) for (var upperX = 0; upperX < height; upperX++) if (grids[level + 1][0][upperX] == '#') neighborBugs++;
                                if (x == 2 && y == 3) for (var upperX = 0; upperX < height; upperX++) if (grids[level + 1][height - 1][upperX] == '#') neighborBugs++;
                                if (y == 2 && x == 1) for (var upperY = 0; upperY < width; upperY++) if (grids[level + 1][upperY][0] == '#') neighborBugs++;
                                if (y == 2 && x == 3) for (var upperY = 0; upperY < width; upperY++) if (grids[level + 1][upperY][width - 1] == '#') neighborBugs++;
                            }

                            if (grids[level][y][x] == '#' && neighborBugs == 1) newGrid[y][x] = '#';
                            else if (grids[level][y][x] == '.' && (neighborBugs == 1 || neighborBugs == 2)) newGrid[y][x] = '#';
                            else newGrid[y][x] = '.';
                        }
                    }
                    newGrids.Add(level, newGrid);
                }
                grids = newGrids;
            }

            int result = 0;
            foreach (var grid in grids) for (var y = 0; y < height; y++) for (var x = 0; x < width; x++) if (grid.Value[y][x] == '#') result++;
            return result.ToString();
        }
    }
}
