using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day23Solver : AbstractSolver
    {
        readonly Dictionary<(int x, int y), char> Map = new();
        readonly Dictionary<char, List<(int x, int y)>> Destinations = new();
        readonly List<(int x, int y)> Buffers = new();

        public override string Part1()
        {
            Dictionary<(int x, int y), char> podsToMove = new();
            Dictionary<(int x, int y), char> podsAtDestination = new();

            Destinations.Add('A', new List<(int x, int y)> { (3, 2), (3, 3) });
            Destinations.Add('B', new List<(int x, int y)> { (5, 2), (5, 3) });
            Destinations.Add('C', new List<(int x, int y)> { (7, 2), (7, 3) });
            Destinations.Add('D', new List<(int x, int y)> { (9, 2), (9, 3) });

            Buffers.Add((1, 1));
            Buffers.Add((2, 1));
            Buffers.Add((4, 1));
            Buffers.Add((6, 1));
            Buffers.Add((8, 1));
            Buffers.Add((10, 1));
            Buffers.Add((11, 1));

            using (StreamReader input = File.OpenText(InputFile))
            {
                string[] lines = input.ReadToEnd().Split("\r\n");
                for (int y = lines.Length - 1; y >= 0; y--)
                {
                    string line = lines[y];
                    int x = 0;
                    foreach (char c in line)
                    {
                        if (c == '.' || c == 'A' || c == 'B' || c == 'C' || c == 'D')
                        {
                            Map.Add((x, y), '.');
                            if (c != '.') podsToMove.Add((x, y), c);
                        }
                        else
                        {
                            Map.Add((x, y), c);
                        }
                        x++;
                    }
                }
            }

            return GetBestSolveCost(podsToMove, podsAtDestination).ToString();
        }

        class CacheComparer : IEqualityComparer<(Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination)>
        {
            public bool Equals((Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination) x,
                               (Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination) y)
            {
                if (x.podsToMove.Count != y.podsToMove.Count) return false;
                foreach ((int x, int y) key in x.podsToMove.Keys)
                {
                    if (!y.podsToMove.ContainsKey(key)) return false;
                    if (x.podsToMove[key] != y.podsToMove[key]) return false;
                }

                return true;
            }

            public int GetHashCode([DisallowNull] (Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination) obj)
            {
                HashCode hash = new();
                foreach (KeyValuePair<(int x, int y), char> entry in obj.podsToMove)
                {
                    hash.Add(entry);
                }
                return hash.ToHashCode();
            }
        }

        readonly Dictionary<(Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination), int> Cache = new(new CacheComparer());
        public int GetBestSolveCost(Dictionary<(int x, int y), char> podsToMove, Dictionary<(int x, int y), char> podsAtDestination)
        {
            //PrintMap(new Dictionary<(int x, int y), char>(podsToMove.Concat(podsAtDestination)));
            if (podsToMove.Count == 0) return 0;
            if (Cache.ContainsKey((podsToMove, podsAtDestination))) return Cache[(podsToMove, podsAtDestination)];
            int bestSolveCost = int.MaxValue;

            foreach (KeyValuePair<(int x, int y), char> pod in podsToMove)
            {
                (int, int) freeDestination = Destinations[pod.Value].Except(podsAtDestination.Keys.Concat(podsToMove.Keys)).LastOrDefault((-1, -1));
                if (freeDestination != (-1, -1) && (
                    freeDestination == Destinations[pod.Value][^1] ||
                    ((freeDestination == Destinations[pod.Value][^2] && podsAtDestination.ContainsKey(Destinations[pod.Value][^1])) ||
                     Destinations[pod.Value].Count > 2 && (freeDestination == Destinations[pod.Value][^3] && (podsAtDestination.ContainsKey(Destinations[pod.Value][^2]) && podsAtDestination.ContainsKey(Destinations[pod.Value][^1]))) ||
                     Destinations[pod.Value].Count > 3 && (freeDestination == Destinations[pod.Value][^4] && (podsAtDestination.ContainsKey(Destinations[pod.Value][^3]) && podsAtDestination.ContainsKey(Destinations[pod.Value][^2]) && podsAtDestination.ContainsKey(Destinations[pod.Value][^1])))
                    )))
                {
                    int cost = PathCostTo(pod.Key, freeDestination, new Dictionary<(int x, int y), char>(podsToMove.Concat(podsAtDestination)), pod.Value);
                    if (cost > -1)
                    {
                        Dictionary<(int x, int y), char> newPodsToMove = new(podsToMove);
                        _ = newPodsToMove.Remove(pod.Key);
                        Dictionary<(int x, int y), char> newPodsAtDestination = new(podsAtDestination);
                        newPodsAtDestination.Add(freeDestination, pod.Value);

                        int subSequentCost = GetBestSolveCost(newPodsToMove, newPodsAtDestination);
                        if (subSequentCost != -1)
                        {
                            cost = (cost * GetMovementCost(pod.Value)) + subSequentCost;
                            bestSolveCost = Math.Min(cost, bestSolveCost);
                        }
                    }
                }
            }
            if (bestSolveCost != int.MaxValue)
            {
                Cache.Add((podsToMove, podsAtDestination), bestSolveCost);
                return bestSolveCost; // don't check movement to buffers if movement to dest was possible
            }

            foreach (KeyValuePair<(int x, int y), char> pod in podsToMove.Where(p => p.Key.y > 1).OrderByDescending(p => p.Key.y))
            {
                foreach ((int x, int y) buffer in Buffers.Except(podsToMove.Keys))
                {
                    int cost = PathCostTo(pod.Key, buffer, new Dictionary<(int x, int y), char>(podsToMove.Concat(podsAtDestination)), pod.Value);
                    if (cost > -1)
                    {

                        Dictionary<(int x, int y), char> newPodsToMove = new(podsToMove);
                        _ = newPodsToMove.Remove(pod.Key);
                        newPodsToMove.Add(buffer, pod.Value);
                        Dictionary<(int x, int y), char> newPodsAtDestination = new(podsAtDestination);

                        int subSequentCost = GetBestSolveCost(newPodsToMove, newPodsAtDestination);
                        if (subSequentCost != -1)
                        {
                            cost = (cost * GetMovementCost(pod.Value)) + subSequentCost;
                            bestSolveCost = Math.Min(cost, bestSolveCost);
                        }
                    }
                }
            }

            if (bestSolveCost != int.MaxValue)
            {
                Cache.Add((podsToMove, podsAtDestination), bestSolveCost);
                return bestSolveCost;
            }
            Cache.Add((podsToMove, podsAtDestination), -1);
            return -1;
        }

        public int PathCostTo((int x, int y) from, (int x, int y) to, Dictionary<(int x, int y), char> pods, char pod, List<(int x, int y)>? visited = null)
        {
            if (from == to) return 0;
            if (visited == null) visited = new List<(int x, int y)>();
            visited.Add(from);
            foreach ((int x, int y) location in GetFreeNeighborLocations(from, pods, pod).Except(visited))
            {
                int costFromLocation = PathCostTo(location, to, pods, pod, visited);
                if (costFromLocation != -1) return costFromLocation + 1;
            }

            return -1;
        }

        public List<(int x, int y)> GetFreeNeighborLocations((int x, int y) location, Dictionary<(int x, int y), char> pods, char pod)
        {
            List<(int x, int y)> freeLocations = new();

            for (int offset = -1; offset <= 1; offset += 2)
            {
                (int x, int y) testLocation = (location.x + offset, location.y);
                if (Map[testLocation] == '.' && !pods.ContainsKey(testLocation)) freeLocations.Add(testLocation);

                testLocation = (location.x, location.y + offset);
                if (Map[testLocation] == '.')
                {
                    if (location.y == 1 && testLocation.y == 2 && Destinations[pod].Contains(testLocation)) freeLocations.Add(testLocation);
                    else if (!pods.ContainsKey(testLocation)) freeLocations.Add(testLocation);
                }
            }

            return freeLocations;
        }

        public static int GetMovementCost(char c)
        {
            return c switch
            {
                'A' => 1,
                'B' => 10,
                'C' => 100,
                'D' => 1000,
                _ => throw new NotImplementedException()
            };
        }

        public void PrintMap(Dictionary<(int x, int y), char> pods)
        {
            StringBuilder sb = new();

            int width = Map.MaxBy(l => l.Key.x).Key.x;
            int height = Map.MaxBy(l => l.Key.y).Key.y;

            for (int y = 0; y < height + 1; y++)
            {
                for (int x = 0; x < width + 1; x++)
                {
                    if (Map[(x, y)] == '.' && pods.ContainsKey((x, y))) _ = sb.Append(pods[(x, y)]);
                    else _ = sb.Append(Map[(x, y)]);

                }
                _ = sb.Append("\r\n");
            }

            Program.PrintData(sb.ToString(), 1, true);
        }


        public override string Part2()
        {
            Dictionary<(int x, int y), char> podsToMove = new();
            Dictionary<(int x, int y), char> podsAtDestination = new();
            Map.Clear();
            Cache.Clear();

            Destinations['A'].Add((3, 4)); Destinations['A'].Add((3, 5));
            Destinations['B'].Add((5, 4)); Destinations['B'].Add((5, 5));
            Destinations['C'].Add((7, 4)); Destinations['C'].Add((7, 5));
            Destinations['D'].Add((9, 4)); Destinations['D'].Add((9, 5));

            using (StreamReader input = File.OpenText(InputFile))
            {
                List<string> lines = new(input.ReadToEnd().Split("\r\n"));
                lines.Insert(3, "  #D#C#B#A#  ");
                lines.Insert(4, "  #D#B#A#C#  ");

                for (int y = lines.Count - 1; y >= 0; y--)
                {
                    string line = lines[y];
                    int x = 0;
                    foreach (char c in line)
                    {
                        if (c == '.' || c == 'A' || c == 'B' || c == 'C' || c == 'D')
                        {
                            Map.Add((x, y), '.');
                            if (c != '.') podsToMove.Add((x, y), c);
                        }
                        else
                        {
                            Map.Add((x, y), c);
                        }
                        x++;
                    }
                }
            }

            return GetBestSolveCost(podsToMove, podsAtDestination).ToString();
        }
    }
}
