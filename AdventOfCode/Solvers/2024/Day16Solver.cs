namespace AdventOfCode.Solvers.Year2024
{
    class Day16Solver : AbstractSolver
    {
        HashSet<(int x, int y)> Maze = [];
        (int x, int y) StartPos;
        (int x, int y) EndPos;


        Dictionary<char, (int x, int y)> Directions = new()
            {
                { 'N', (0, -1) },
                { 'E', (1, 0) },
                { 'S', (0, 1) },
                { 'W', (-1, 0) }
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
                        if (c == 'S') StartPos = (x, y);
                        else if (c == 'E') EndPos = (x, y);
                        else if (c == '#') Maze.Add((x, y));
                        x++;
                    }
                    y++;
                }
            }

            Dictionary<((int x, int y) pos, char orientation), int> costs = new()
            {
                { (StartPos, 'E'), 0 }
            };
            PriorityQueue<((int x, int y) pos, char orientation), int> queue = new();
            queue.Enqueue((StartPos, 'E'), 0);

            int result = int.MaxValue;
            while (queue.Count > 0)
            {
                ((int x, int y) pos, char orientation) currentLocation = queue.Dequeue();
                if (currentLocation.pos == EndPos)
                {
                    result = Math.Min(result, costs[currentLocation]);
                }
                else
                {
                    foreach (((int x, int y) pos, char orientation, int cost) nextMove in GetNextMoves(currentLocation, costs[currentLocation]))
                    {
                        if (costs.TryGetValue((nextMove.pos, nextMove.orientation), out int nextCost) && nextCost < nextMove.cost) continue;
                        if (!costs.TryAdd((nextMove.pos, nextMove.orientation), nextMove.cost)) costs[(nextMove.pos, nextMove.orientation)] = nextMove.cost;
                        queue.Enqueue((nextMove.pos, nextMove.orientation), nextMove.cost);
                    }
                }
            }

            return result.ToString();
        }


        private (int x, int y) AddPos((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);


        public override string Part2()
        {
            Dictionary<((int x, int y) pos, char orientation), int> costs = new()
            {
                { (StartPos, 'E'), 0 }
            };
            Dictionary<((int x, int y) pos, char orientation), HashSet<((int x, int y) pos, char orientation)>> previousLocations = new()
            {
                { (StartPos, 'E'), []}
            };
            PriorityQueue<((int x, int y) pos, char orientation), int> queue = new();
            queue.Enqueue((StartPos, 'E'), 0);

            int result = int.MaxValue;
            while (queue.Count > 0)
            {
                ((int x, int y) pos, char orientation) currentLocation = queue.Dequeue();
                if (currentLocation.pos == EndPos)
                {
                    result = Math.Min(result, costs[currentLocation]);
                }
                else
                {
                    foreach (((int x, int y) pos, char orientation, int cost) nextMove in GetNextMoves(currentLocation, costs[currentLocation]))
                    {
                        if (costs.TryGetValue((nextMove.pos, nextMove.orientation), out int nextCost) && nextCost < nextMove.cost) continue;
                        else if (nextCost == nextMove.cost) previousLocations[(nextMove.pos, nextMove.orientation)].Add(currentLocation);
                        else
                        {
                            if (!costs.TryAdd((nextMove.pos, nextMove.orientation), nextMove.cost)) costs[(nextMove.pos, nextMove.orientation)] = nextMove.cost;
                            if (!previousLocations.TryAdd((nextMove.pos, nextMove.orientation), [currentLocation])) previousLocations[(nextMove.pos, nextMove.orientation)] = [currentLocation];
                        }
                        queue.Enqueue((nextMove.pos, nextMove.orientation), nextMove.cost);
                    }
                }
            }

            HashSet<(int x, int y)> locationsOnBestPaths = [EndPos];
            Stack<((int x, int y) pos, char orientation)> backtrackingLocations = [];
            if (costs.TryGetValue((EndPos, 'N'), out int pathCost) && pathCost == result) backtrackingLocations.Push((EndPos, 'N'));
            if (costs.TryGetValue((EndPos, 'E'), out pathCost) && pathCost == result) backtrackingLocations.Push((EndPos, 'E'));

            while (backtrackingLocations.Count > 0)
            {
                ((int x, int y) pos, char orientation) currentBacktrackLocation = backtrackingLocations.Pop();
                foreach (((int x, int y) pos, char orientation) location in previousLocations[currentBacktrackLocation])
                {
                    locationsOnBestPaths.Add(location.pos);
                    backtrackingLocations.Push(location);
                }
            }

            return locationsOnBestPaths.Count.ToString();
        }

        private List<((int x, int y) pos, char orientation, int cost)> GetNextMoves(((int x, int y) pos, char orientation) location, int cost)
        {
            List<((int x, int y) pos, char orientation, int cost)> nextMoves = [];
            (int x, int y) straight = AddPos(location.pos, Directions[location.orientation]);
            if (!Maze.Contains(straight)) nextMoves.Add((straight, location.orientation, cost + 1));
            char[] turns = ['N', 'S'];
            if (location.orientation == 'N' || location.orientation == 'S') turns = ['W', 'E'];
            foreach (char turn in turns) nextMoves.Add((location.pos, turn, cost + 1000));
            return nextMoves;
        }
    }
}