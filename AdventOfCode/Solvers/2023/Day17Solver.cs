namespace AdventOfCode.Solvers.Year2023
{
    class Day17Solver : AbstractSolver
    {
        Dictionary<(int x, int y), int> map = new();
        int height = 0;
        int width = 0;

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
                        map.Add((x, y), int.Parse(line[x].ToString()));
                    }
                    y++;
                }
                height = map.Max(m => m.Key.y) + 1;
                width = map.Max(m => m.Key.x) + 1;
            }

            return NavigateToEnd((0, 0), 0).ToString();
        }

        public int NavigateToEnd((int x, int y) pos, int straightMoves, char direction = ' ', bool part2 = false)
        {
            PriorityQueue<((int x, int y) pos, char direction, int straightMoves), int> queue = new();
            Dictionary<((int x, int y) pos, char direction, int straightMoves), (int cost, ((int x, int y) pos, char direction, int straightMoves) prev)> cost = new();

            cost[(pos, ' ', 0)] = (0, (pos, ' ', 0));
            queue.Enqueue((pos, ' ', 0), 0);
            while (queue.Count > 0)
            {
                ((int x, int y) pos, char direction, int straightMoves) move = queue.Dequeue();

                foreach (var nextMove in GetPossibleMoves(move.pos, move.direction, move.straightMoves, part2))
                {
                    if (!cost.ContainsKey(nextMove))
                    {
                        cost[nextMove] = (map[nextMove.pos] + cost[move].cost, move);
                        queue.Enqueue(nextMove, cost[nextMove].cost);
                    }
                    else if (cost[nextMove].cost > map[nextMove.pos] + cost[move].cost)
                    {
                        cost[nextMove] = (map[nextMove.pos] + cost[move].cost, move);
                        queue.Enqueue(nextMove, cost[nextMove].cost);
                    }
                }
            }

            if(!part2) return cost.Where(c => c.Key.pos == (width - 1, height - 1)).Min(c => c.Value.cost);
            else return cost.Where(c => c.Key.pos == (width - 1, height - 1) && c.Key.straightMoves >= 4).Min(c => c.Value.cost);
        }

        public List<((int x, int y) pos, char direction, int straightMoves)> GetPossibleMoves((int x, int y) pos, char direction, int straightMoves, bool part2)
        {
            List<((int x, int y) pos, char direction, int straightMoves)> possibleMoves = new();
            switch (direction)
            {
                case 'e':
                    if(!part2 || straightMoves > 3) possibleMoves.Add(((pos.x, pos.y - 1), 'n', 1));
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x, pos.y + 1), 's', 1));
                    if ((!part2 && straightMoves < 3) || (part2 && straightMoves < 10)) possibleMoves.Add(((pos.x + 1, pos.y), 'e', straightMoves + 1));
                    break;
                case 'w':
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x, pos.y - 1), 'n', 1));
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x, pos.y + 1), 's', 1));
                    if ((!part2 && straightMoves < 3) || (part2 && straightMoves < 10)) possibleMoves.Add(((pos.x - 1, pos.y), 'w', straightMoves + 1));
                    break;
                case 'n':
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x - 1, pos.y), 'w', 1));
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x + 1, pos.y), 'e', 1));
                    if ((!part2 && straightMoves < 3) || (part2 && straightMoves < 10)) possibleMoves.Add(((pos.x, pos.y - 1), 'n', straightMoves + 1));
                    break;
                case 's':
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x - 1, pos.y), 'w', 1));
                    if (!part2 || straightMoves > 3) possibleMoves.Add(((pos.x + 1, pos.y), 'e', 1));
                    if ((!part2 && straightMoves < 3) || (part2 && straightMoves < 10)) possibleMoves.Add(((pos.x, pos.y + 1), 's', straightMoves + 1));
                    break;
                case ' ':
                    possibleMoves.Add(((pos.x, pos.y - 1), 'n', 1));
                    possibleMoves.Add(((pos.x, pos.y + 1), 's', 1));
                    possibleMoves.Add(((pos.x - 1, pos.y), 'w', 1));
                    possibleMoves.Add(((pos.x + 1, pos.y), 'e', 1));
                    break;
            }
            possibleMoves.RemoveAll(p => p.pos.x < 0 || p.pos.x >= width || p.pos.y < 0 || p.pos.y >= height);

            return possibleMoves;
        }

        public override string Part2()
        {
            return NavigateToEnd((0, 0), 0, part2: true).ToString();
        }
    }
}
