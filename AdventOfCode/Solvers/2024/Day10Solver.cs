namespace AdventOfCode.Solvers.Year2024
{
    class Day10Solver : AbstractSolver
    {
        Dictionary<(int x, int y), int> Map = [];
        (int x, int y) MapSize;

        HashSet<(int x, int y)> TrailHeads = [];

        Dictionary<string, (int x, int y)> Directions = new()
            {
                { "N", (0, -1) },
                { "E", (1, 0) },
                { "S", (0, 1) },
                { "W", (-1, 0) },
            };

        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach (char c in line)
                    {
                        Map.Add((x, y), c - 48);
                        if (c - 48 == 0) TrailHeads.Add((x, y));
                        x++;
                    }
                    y++;
                    MapSize.x = x;
                }
                MapSize.y = y;
            }

            int result = 0;
            foreach ((int x, int y) trailhead in TrailHeads) result += ReachableTrailEnds(trailhead).Count;
            return result.ToString();
        }

        private HashSet<(int x, int y)> ReachableTrailEnds((int x, int y) pos)
        {
            HashSet<(int x, int y)> reachableEnds = [];

            if (Map[pos] == 9) reachableEnds.Add(pos);
            else
            {
                foreach ((int x, int y) direction in Directions.Values)
                {
                    (int x, int y) neighbor = AddPos(pos, direction);
                    if (OnMap(neighbor) && Map[neighbor] == Map[pos] + 1) reachableEnds.UnionWith(ReachableTrailEnds(neighbor));
                }
            }
            return reachableEnds;
        }

        private (int x, int y) AddPos((int x, int y) a, (int x, int y) b)
        {
            return (a.x + b.x, a.y + b.y);
        }

        private bool OnMap((int x, int y) pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= MapSize.x || pos.y >= MapSize.y) return false;
            return true;
        }

        private int ValidTrailsFrom((int x, int y) pos)
        {
            if (Map[pos] == 9) return 1;

            int validTrails = 0;
            foreach ((int x, int y) direction in Directions.Values)
            {
                (int x, int y) neighbor = AddPos(pos, direction);
                if (OnMap(neighbor) && Map[neighbor] == Map[pos] + 1) validTrails += ValidTrailsFrom(neighbor);
            }
            return validTrails;
        }

        public override string Part2()
        {
            int result = 0;
            foreach ((int x, int y) trailhead in TrailHeads) result += ValidTrailsFrom(trailhead);
            return result.ToString();
        }
    }
}
