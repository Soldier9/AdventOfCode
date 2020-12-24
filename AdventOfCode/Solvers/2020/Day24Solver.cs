using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day24Solver : AbstractSolver
    {
        HashSet<(double, int)> BlackTiles = new HashSet<(double, int)>();
        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                Regex getDirections = new Regex(@"se|sw|ne|nw|e|w");
                while (!input.EndOfStream)
                {
                    (double, int) pos = (0, 0);
                    foreach (Match movement in getDirections.Matches(input.ReadLine()))
                    {
                        switch (movement.Value)
                        {
                            case "se": pos = (pos.Item1 + 0.5, pos.Item2 + 1); break;
                            case "sw": pos = pos = (pos.Item1 - 0.5, pos.Item2 + 1); break;
                            case "ne": pos = (pos.Item1 + 0.5, pos.Item2 - 1); break;
                            case "nw": pos = pos = (pos.Item1 - 0.5, pos.Item2 - 1); break;
                            case "e": pos = (pos.Item1 + 1, pos.Item2); break;
                            case "w": pos = pos = (pos.Item1 - 1, pos.Item2); break;
                        }
                    }
                    if (BlackTiles.Contains(pos)) BlackTiles.Remove(pos);
                    else BlackTiles.Add(pos);
                }
            }

            return BlackTiles.Count.ToString();
        }

        public override string Part2()
        {
            List<(double, int)> neighbors = new List<(double, int)>
            {
                (-0.5, -1),
                (+0.5, -1),
                (-1, 0),
                (1, 0),
                (-0.5, 1),
                (0.5, 1)
            };

            for (int d = 0; d < 100; d++)
            {
                HashSet<(double, int)> newLayout = new HashSet<(double, int)>();
                HashSet<(double, int)> interestingTiles = new HashSet<(double, int)>();

                foreach ((double, int) pos in BlackTiles)
                {
                    foreach ((double, int) n in neighbors.Select(n => (n.Item1 + pos.Item1, n.Item2 + pos.Item2)).ToList()) interestingTiles.Add(n);
                    interestingTiles.Add(pos);
                }

                foreach ((double, int) pos in interestingTiles)
                {
                    int blackNeighbors = 0;
                    foreach ((double, int) n in neighbors.Select(n => (n.Item1 + pos.Item1, n.Item2 + pos.Item2)).ToList()) if (BlackTiles.Contains(n)) blackNeighbors++;

                    if (BlackTiles.Contains(pos))
                    {
                        if (!(blackNeighbors == 0 || blackNeighbors > 2)) newLayout.Add(pos);
                    }
                    else
                    {
                        if (blackNeighbors == 2) newLayout.Add(pos);
                    }
                }

                BlackTiles = newLayout;
            }

            return BlackTiles.Count.ToString();
        }
    }
}