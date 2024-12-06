namespace AdventOfCode.Solvers.Year2024
{
    class Day6Solver : AbstractSolver
    {
        HashSet<(int x, int y)> Obstacles = [];
        int MaxX, MaxY;
        (int x, int y) StartPos;

        Dictionary<char, (int x, int y)> Directions = new Dictionary<char, (int x, int y)>
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
                    int x = 0;
                    string line = input.ReadLine()!;
                    foreach (char c in line)
                    {
                        if (c == '#') Obstacles.Add((x, y));
                        else if (c == '^') StartPos = (x, y);
                        x++;
                    }
                    y++;
                    MaxX = x - 1;
                }
                MaxY = y - 1;
            }

            HashSet<(int x, int y)> visited = [StartPos];
            (int x, int y) currentPos = StartPos;
            char facing = 'N';
            while (currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x <= MaxX && currentPos.y <= MaxY)
            {
                List<(int x, int y)> nextStretch = AdvanceToObstacle(currentPos, facing);
                visited.UnionWith(nextStretch);
                currentPos = nextStretch[^1];
                switch (facing)
                {
                    case 'N': facing = 'E'; break;
                    case 'E': facing = 'S'; break;
                    case 'S': facing = 'W'; break;
                    case 'W': facing = 'N'; break;
                }
            }
            visited.Remove(currentPos); // this is outside the area

            return visited.Count.ToString();
        }

        List<(int x, int y)> AdvanceToObstacle((int x, int y) from, char direction)
        {
            List<(int x, int y)> pathTraveled = [];

            while (from.x >= 0 && from.y >= 0 && from.x <= MaxX && from.y <= MaxY)
            {
                from = AddPos(from, Directions[direction]);
                if (Obstacles.Contains(from)) return pathTraveled;
                pathTraveled.Add(from);
            }
            return pathTraveled;
        }
        private (int x, int y) AddPos((int x, int y) pos1, (int x, int y) pos2)
        {
            return (pos1.x + pos2.x, pos1.y + pos2.y);
        }


        public override string Part2()
        {
            HashSet<((int x, int y) pos, char direction)> visited = [(StartPos, 'N')];
            (int x, int y) currentPos = StartPos;
            char facing = 'N';
            while (currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x <= MaxX && currentPos.y <= MaxY)
            {
                List<(int x, int y)> nextStretch = AdvanceToObstacle(currentPos, facing);
                visited.UnionWith(nextStretch.Select(pos => (pos, facing)));

                currentPos = nextStretch[^1];
                switch (facing)
                {
                    case 'N': facing = 'E'; break;
                    case 'E': facing = 'S'; break;
                    case 'S': facing = 'W'; break;
                    case 'W': facing = 'N'; break;
                }
            }
            foreach (char direction in Directions.Keys) visited.Remove((currentPos, direction)); // this is outside the area

            HashSet<(int x, int y)> orgObstacles = new(Obstacles);
            HashSet<(int x, int y)> newObstacles = [];
            foreach ((int x, int y) potentialObstaclePosition in visited.Select(p => p.pos))
            {
                if (potentialObstaclePosition == StartPos) continue;

                Obstacles = new(orgObstacles)
                {
                    potentialObstaclePosition
                };
                currentPos = StartPos;
                facing = 'N';
                visited = [(StartPos, 'N')];

                while (currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x <= MaxX && currentPos.y <= MaxY)
                {
                    List<(int x, int y)> nextStretch = AdvanceToObstacle(currentPos, facing);
                    if (nextStretch.Count > 0)
                    {
                        currentPos = nextStretch[^1];
                        if (visited.Contains((currentPos, facing)))
                        {
                            newObstacles.Add(potentialObstaclePosition);
                            break;
                        }
                    }
                    visited.UnionWith(nextStretch.Select(pos => (pos, facing)));

                    switch (facing)
                    {
                        case 'N': facing = 'E'; break;
                        case 'E': facing = 'S'; break;
                        case 'S': facing = 'W'; break;
                        case 'W': facing = 'N'; break;
                    }
                }
            }

            return newObstacles.Count.ToString();
        }
    }
}
