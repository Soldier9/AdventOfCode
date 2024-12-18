namespace AdventOfCode.Solvers.Year2024
{
    class Day18Solver : AbstractSolver
    {
        HashSet<(int x, int y)> Grid = [];
        int GridSize = 71;
        List<(int x, int y)> Bytes = [];

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    int[] line = input.ReadLine()!.Split(',').Select(n => int.Parse(n)).ToArray();
                    Bytes.Add((line[0], line[1]));
                }
            }

            for (int i = 0; i < 1024; i++) Grid.Add(Bytes[i]);

            int result = NavigateGrid((0, 0), (70, 70)).cost;
            return result.ToString();
        }

        List<(int x, int y)> Directions =
        [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];
        (int x, int y) AddPos((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
        public (int cost, List<(int x, int y)> path) NavigateGrid((int x, int y) from, (int x, int y) to)
        {
            Dictionary<(int x, int y), int> costs = [];
            Dictionary<(int x, int y), (int x, int y)> previous = [];
            PriorityQueue<(int x, int y), int> queue = new();
            queue.Enqueue(from, 0);
            costs.Add(from, 0);

            while (queue.Count > 0)
            {
                (int x, int y) currentLocation = queue.Dequeue();
                int nextCost = costs[currentLocation] + 1;
                foreach ((int x, int y) direction in Directions)
                {
                    (int x, int y) neighbor = AddPos(currentLocation, direction);
                    if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= GridSize || neighbor.y >= GridSize) continue;
                    if (Grid.Contains(neighbor)) continue;

                    if (!costs.TryGetValue(neighbor, out int value) || value > nextCost)
                    {
                        if (!costs.TryAdd(neighbor, nextCost)) costs[neighbor] = nextCost;
                        if (!previous.TryAdd(neighbor, currentLocation)) previous[neighbor] = currentLocation;
                        queue.Enqueue(neighbor, nextCost);
                    }
                }
            }

            if (!costs.ContainsKey(to)) return (0, []);

            List<(int x, int y)> path = [];
            path.Add(to);
            while (path[^1] != from) path.Add(previous[path[^1]]);
            path.Reverse();

            return (costs[to], path);
        }


        public override string Part2()
        {
            HashSet<(int x, int y)> LastPath = new(NavigateGrid((0, 0), (70, 70)).path);
            for (int i = 1024; i < Bytes.Count; i++)
            {
                Grid.Add(Bytes[i]);
                if (LastPath.Contains(Bytes[i]))
                {
                    LastPath = new(NavigateGrid((0, 0), (70, 70)).path);
                    if (LastPath.Count == 0) return Bytes[i].x.ToString() + "," + Bytes[i].y.ToString();
                }
            }

            return "".ToString();
        }
    }
}
