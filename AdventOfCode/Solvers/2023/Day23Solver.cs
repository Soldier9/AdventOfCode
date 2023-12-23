namespace AdventOfCode.Solvers.Year2023
{
    class Day23Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> map = new();
        (int x, int y) StartingPos = (-1, -1);
        (int x, int y) DestinationPos = (-1, -1);
        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    for (int x = 0; x < line.Length; x++)
                    {
                        if (StartingPos == (-1, -1) && line[x] == '.')
                        {
                            StartingPos = (x, y);
                        }
                        else if (".<>^v".Contains(line[x]))
                        {
                            map.Add((x, y), line[x]);
                            if (line[x] == '.') DestinationPos = (x, y);
                        }
                    }
                    y++;
                }
            }

            Dictionary<(int x, int y), long> visited = new();
            visited.Add(StartingPos, 0);
            return DistanceToEnd(StartingPos, visited).ToString();
        }

        long DistanceToEnd((int x, int y) fromPos, Dictionary<(int x, int y), long> visited)
        {
            if (fromPos == DestinationPos) return visited[fromPos];

            long longest = 0;
            foreach ((int x, int y) pos in GetMoves(fromPos))
            {
                if (!visited.ContainsKey(pos))
                {
                    Dictionary<(int x, int y), long> recursiveVisited = new(visited);
                    recursiveVisited.Add(pos, visited[fromPos] + 1);
                    longest = Math.Max(longest, DistanceToEnd(pos, recursiveVisited));
                }
            }
            return longest;
        }

        List<(int x, int y)> GetMoves((int x, int y) pos)
        {
            List<(int x, int y)> moves = new();
            if (map.ContainsKey((pos.x + 1, pos.y)) && (map[(pos.x + 1, pos.y)] == '.' || map[(pos.x + 1, pos.y)] == '>')) moves.Add((pos.x + 1, pos.y));
            if (map.ContainsKey((pos.x - 1, pos.y)) && (map[(pos.x - 1, pos.y)] == '.' || map[(pos.x - 1, pos.y)] == '<')) moves.Add((pos.x - 1, pos.y));
            if (map.ContainsKey((pos.x, pos.y + 1)) && (map[(pos.x, pos.y + 1)] == '.' || map[(pos.x, pos.y + 1)] == 'v')) moves.Add((pos.x, pos.y + 1));
            if (map.ContainsKey((pos.x, pos.y - 1)) && (map[(pos.x, pos.y - 1)] == '.' || map[(pos.x, pos.y - 1)] == '^')) moves.Add((pos.x, pos.y - 1));

            return moves;
        }

        public override string Part2()
        {
            int width = map.Max(m => m.Key.x) + 1;
            int height = map.Max(m => m.Key.y) + 1;

            Dictionary<(int x, int y), List<((int x, int y) dest, int cost)>> nodes = new();
            nodes.Add(StartingPos, new());
            nodes.Add(DestinationPos, new());

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map.ContainsKey((x, y)) && GetMovesPart2((x, y)).Count > 2)
                    {
                        nodes.Add((x, y), new());
                    }
                }
            }

            foreach (KeyValuePair<(int x, int y), List<((int x, int y) dest, int cost)>> node in nodes)
            {
                foreach ((int x, int y) path in GetMovesPart2(node.Key))
                {
                    HashSet<(int x, int y)> visited = new();
                    visited.Add(node.Key);
                    (int x, int y) currentPos = path;
                    int cost = 1;
                    while (!nodes.ContainsKey(currentPos) && currentPos != (-1, -1))
                    {
                        visited.Add(currentPos);
                        currentPos = GetMovesPart2(currentPos).Where(m => !visited.Contains(m)).SingleOrDefault((-1, -1));
                        cost++;
                    }
                    if (currentPos != (-1, -1)) node.Value.Add((currentPos, cost));
                }
            }

            Queue<((int x, int y) pos, Dictionary<(int x, int y), int> visited)> queue = new();
            Dictionary<(int x, int y), int> visitedNodes = new();
            visitedNodes.Add(StartingPos, 0);
            int result = 0;

            queue.Enqueue((StartingPos, visitedNodes));
            while (queue.Count > 0)
            {
                ((int x, int y) pos, Dictionary<(int x, int y), int> visited) from = queue.Dequeue();
                if (from.pos == DestinationPos)
                {
                    result = Math.Max(from.visited[from.pos], result);
                    continue;
                }

                foreach (((int x, int y) dest, int cost) path in nodes[from.pos])
                {
                    if (!from.visited.ContainsKey(path.dest))
                    {
                        visitedNodes = new(from.visited);
                        visitedNodes.Add(path.dest, from.visited[from.pos] + path.cost);
                        queue.Enqueue((path.dest, visitedNodes));
                    }
                }
            }

            return result.ToString();
        }

        List<(int x, int y)> GetMovesPart2((int x, int y) pos)
        {
            List<(int x, int y)> moves = new();
            if (map.ContainsKey((pos.x + 1, pos.y)) && (map[(pos.x + 1, pos.y)] != '#')) moves.Add((pos.x + 1, pos.y));
            if (map.ContainsKey((pos.x - 1, pos.y)) && (map[(pos.x - 1, pos.y)] != '#')) moves.Add((pos.x - 1, pos.y));
            if (map.ContainsKey((pos.x, pos.y + 1)) && (map[(pos.x, pos.y + 1)] != '#')) moves.Add((pos.x, pos.y + 1));
            if (map.ContainsKey((pos.x, pos.y - 1)) && (map[(pos.x, pos.y - 1)] != '#')) moves.Add((pos.x, pos.y - 1));
            return moves;
        }
    }
}
