namespace AdventOfCode.Solvers.Year2023
{
    class Day11Solver : AbstractSolver
    {
        Dictionary<int, (long x, long y)> originalUniverse = new();
        HashSet<(int g1, int g2)> galaxyPairs = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                int galaxyNum = 1;
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    long x = 0;
                    foreach (char c in line)
                    {
                        if (c == '#') originalUniverse.Add(galaxyNum++, (x, y));
                        x++;
                    }
                    y++;
                }
            }

            foreach (KeyValuePair<int, (long x, long y)> g1 in originalUniverse)
            {
                foreach (KeyValuePair<int, (long x, long y)> g2 in originalUniverse.Where(g => g.Key > g1.Key))
                {
                    galaxyPairs.Add((g1.Key, g2.Key));
                }
            }

            long result = 0;
            Dictionary<int, (long x, long y)> expandedUniverse = Expand(originalUniverse, 2);
            foreach ((int g1, int g2) galaxyPair in galaxyPairs)
            {
                result += GetDist(expandedUniverse[galaxyPair.g1], expandedUniverse[galaxyPair.g2]);
            }

            return result.ToString();
        }

        public static long GetDist((long x, long y) pos1, (long x, long y) pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
        }

        public static Dictionary<int, (long x, long y)> Expand(Dictionary<int, (long x, long y)> universe, int factor)
        {
            Dictionary<int, (long x, long y)> expandedUniverse = universe.ToDictionary(g => g.Key, g => g.Value);

            List<long> expandX = new();
            for (long x = 0; x <= universe.Max(g => g.Value.x); x++)
            {
                bool hasGalaxy = false;
                for (long y = 0; y <= universe.Max(g => g.Value.y); y++)
                {
                    if (universe.ContainsValue((x, y)))
                    {
                        hasGalaxy = true;
                        break;
                    }
                }
                if (!hasGalaxy) expandX.Add(x);
            }

            List<long> expandY = new();
            for (long y = 0; y <= universe.Max(g => g.Value.y); y++)
            {
                bool hasGalaxy = false;
                for (long x = 0; x <= universe.Max(g => g.Value.x); x++)
                {
                    if (universe.ContainsValue((x, y)))
                    {
                        hasGalaxy = true;
                        break;
                    }
                }
                if (!hasGalaxy) expandY.Add(y);
            }

            for (int i = expandX.Count - 1; i >= 0; i--)
            {
                foreach (KeyValuePair<int, (long x, long y)> galaxy in expandedUniverse)
                {
                    if (galaxy.Value.x >= expandX[i]) expandedUniverse[galaxy.Key] = (galaxy.Value.x + factor - 1, galaxy.Value.y);
                }
            }
            for (int i = expandY.Count - 1; i >= 0; i--)
            {
                foreach (KeyValuePair<int, (long x, long y)> galaxy in expandedUniverse)
                {
                    if (galaxy.Value.y >= expandY[i]) expandedUniverse[galaxy.Key] = (galaxy.Value.x, galaxy.Value.y + factor - 1);
                }
            }
            return expandedUniverse;
        }

        public override string Part2()
        {
            long result = 0;
            Dictionary<int, (long x, long y)> expandedUniverse = Expand(originalUniverse, 1000000);
            foreach ((int g1, int g2) galaxyPair in galaxyPairs)
            {
                result += GetDist(expandedUniverse[galaxyPair.g1], expandedUniverse[galaxyPair.g2]);
            }

            return result.ToString();
        }
    }
}
