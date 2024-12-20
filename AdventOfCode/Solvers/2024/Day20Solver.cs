namespace AdventOfCode.Solvers.Year2024
{
    class Day20Solver : AbstractSolver
    {
        HashSet<(int x, int y)> Walls = [];
        (int x, int y) StartPos;
        (int x, int y) EndPos;
        Dictionary<(int x, int y), int> PathWithNoCheats = [];

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
                        switch (c)
                        {
                            case '#': Walls.Add((x, y)); break;
                            case 'S': StartPos = (x, y); break;
                            case 'E': EndPos = (x, y); break;
                        }
                        x++;
                    }
                    y++;
                }
            }

            PathWithNoCheats = NavigateGrid(StartPos, EndPos);
            Dictionary<((int x, int y) cheatPos1, (int x, int y) cheatPos2), int> possibleCheats = [];
            foreach ((int x, int y) pos in PathWithNoCheats.Keys)
            {
                foreach ((int x, int y) neighborWall in Directions.Select(n => AddPos(n, pos)).Where(n => Walls.Contains(n)))
                {
                    foreach ((int x, int y) neighborTrack in Directions.Select(n => AddPos(n, neighborWall)).Where(n => PathWithNoCheats.ContainsKey(n)))
                    {
                        if (neighborTrack != pos && PathWithNoCheats[neighborTrack] > PathWithNoCheats[pos]) possibleCheats.TryAdd((neighborWall, neighborTrack), PathWithNoCheats[neighborTrack] - PathWithNoCheats[pos] - 2);
                    }
                }
            }

            int result = possibleCheats.Where(c => c.Value >= 100).Count();
            return result.ToString();
        }


        List<(int x, int y)> Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];
        (int x, int y) AddPos((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
        public Dictionary<(int x, int y), int> NavigateGrid((int x, int y) from, (int x, int y) to)
        {
            Dictionary<(int x, int y), int> costs = [];
            Dictionary<(int x, int y), (int x, int y)> previous = [];
            PriorityQueue<(int x, int y), int> queue = new();
            (int x, int y) currentLocation;

            queue.Enqueue(from, 0);
            costs.Add(from, 0);
            
            while (queue.Count > 0)
            {
                currentLocation = queue.Dequeue();
                int nextCost = costs[currentLocation] + 1;
                
                foreach ((int x, int y) neighbor in Directions.Select(n => AddPos(n, currentLocation)))
                {
                    if (Walls.Contains(neighbor)) continue;
                    if (!costs.TryGetValue(neighbor, out int value) || value > nextCost)
                    {
                        if (!costs.TryAdd(neighbor, nextCost)) costs[neighbor] = nextCost;
                        if (!previous.TryAdd(neighbor, currentLocation)) previous[neighbor] = currentLocation;
                        queue.Enqueue(neighbor, nextCost);
                    }
                }
            }

            if (!costs.ContainsKey(to)) return [];

            Dictionary<(int x, int y), int> path = [];
            currentLocation = to;
            while (currentLocation != from)
            {
                path.Add(currentLocation, costs[currentLocation]);
                currentLocation = previous[currentLocation];
            } 
            path.Add(from, 0);
            return path.Reverse().ToDictionary();
        }

        public override string Part2()
        {
            Dictionary<((int x, int y) cheatStart, (int x, int y) cheatEnd), int> possibleCheats = [];
            foreach (KeyValuePair<(int x, int y), int> cheatFrom in PathWithNoCheats)
            {
                foreach (KeyValuePair<(int x, int y), int> cheatTo in PathWithNoCheats.Where(p => p.Value > cheatFrom.Value))
                {
                    int cheatDist = GetDist(cheatFrom.Key, cheatTo.Key);
                    if (cheatDist <= 20 && cheatDist < (cheatTo.Value - cheatFrom.Value))
                    {
                        possibleCheats.Add((cheatFrom.Key, cheatTo.Key), (cheatTo.Value - cheatFrom.Value) - cheatDist);
                    }
                }
            }

            int result = possibleCheats.Where(c => c.Value >= 100).Count();
            return result.ToString();
        }

        int GetDist((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }
}
