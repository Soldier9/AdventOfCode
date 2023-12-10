namespace AdventOfCode.Solvers.Year2023
{
    class Day10Solver : AbstractSolver
    {
        public override bool HasVisualization => true;

        Dictionary<(int x, int y), char> grid = new();
        int width = 0;
        int height = 0;
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
                        grid.Add((x++, y), c);
                    }
                    y++;
                    width = x;
                }
                height = y;
            }

            return GetFurthestPoint(grid.Single(g => g.Value == 'S').Key).ToString();
        }

        public int GetFurthestPoint((int x, int y) pos)
        {
            return GetCycleFrom(pos).Max(p => p.Value);
        }

        public Dictionary<(int x, int y), int> GetCycleFrom((int x, int y) pos)
        {
            Dictionary<(int x, int y), int> testedPoints = new();
            testedPoints.Add(pos, 0);

            Queue<(int x, int y)> toTest = new();
            (int x, int y) testPos = pos;
            do
            {
                foreach ((int x, int y) neighbor in GetConnections(testPos))
                {
                    if (!testedPoints.ContainsKey(neighbor))
                    {
                        testedPoints.Add(neighbor, testedPoints[testPos] + 1);
                        toTest.Enqueue(neighbor);
                    }
                }
            } while (toTest.TryDequeue(out testPos));
            return testedPoints;
        }

        public List<(int x, int y)> GetConnections((int x, int y) pos)
        {
            List<(int x, int y)> connections = new();
            foreach ((int x, int y) connection in GetPossibleConnections(pos))
            {
                if (GetPossibleConnections(connection).Contains(pos)) connections.Add(connection);
            }
            return connections;
        }

        private List<(int x, int y)> GetPossibleConnections((int x, int y) pos)
        {
            List<(int x, int y)> possibleConnections = new();
            switch (grid[pos])
            {
                case '|':
                    possibleConnections.Add((pos.x, pos.y - 1));
                    possibleConnections.Add((pos.x, pos.y + 1));
                    break;
                case '-':
                    possibleConnections.Add((pos.x - 1, pos.y));
                    possibleConnections.Add((pos.x + 1, pos.y));
                    break;
                case 'L':
                    possibleConnections.Add((pos.x, pos.y - 1));
                    possibleConnections.Add((pos.x + 1, pos.y));
                    break;
                case 'J':
                    possibleConnections.Add((pos.x, pos.y - 1));
                    possibleConnections.Add((pos.x - 1, pos.y));
                    break;
                case '7':
                    possibleConnections.Add((pos.x - 1, pos.y));
                    possibleConnections.Add((pos.x, pos.y + 1));
                    break;
                case 'F':
                    possibleConnections.Add((pos.x + 1, pos.y));
                    possibleConnections.Add((pos.x, pos.y + 1));
                    break;
                case '.':
                    break;
                case 'S':
                    possibleConnections.Add((pos.x, pos.y - 1));
                    possibleConnections.Add((pos.x, pos.y + 1));
                    possibleConnections.Add((pos.x - 1, pos.y));
                    possibleConnections.Add((pos.x + 1, pos.y));
                    break;
            }
            possibleConnections.RemoveAll(c => c.x < 0 || c.x >= width || c.y < 0 || c.y >= height);
            return possibleConnections;
        }


        public override string Part2()
        {
            HashSet<(int x, int y)> cycle = GetCycleFrom(grid.Single(g => g.Value == 'S').Key).Select(c => c.Key).ToHashSet();
            grid[grid.Single(g => g.Value == 'S').Key] = '-'; // works for my specific input, might need changing for other inputs!

            Dictionary<(int x, int y), char> pipes = new();
            if (Program.VisualizationEnabled)
            {
                foreach ((int x, int y) pos in cycle)
                {
                    pipes.Add(pos, grid[pos]);
                }
            }
            
            int result = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!cycle.Contains((x, y)) && GetVerticalCycleCrosses((x, y), cycle) % 2 == 1)
                    {
                        result++;
                        if(Program.VisualizationEnabled) pipes.Add((x, y), '#');
                    }
                }
            }

            if (Program.VisualizationEnabled)
            {
                Dictionary<char, string> decorations = new Dictionary<char, string>();
                decorations.Add('#', "\u001b[48;5;250m");
                Program.PrintData(Program.CreateStringFromDict(pipes, decorations), 0, false, true);
            }
            return result.ToString();
        }

        public int GetVerticalCycleCrosses((int x, int y) pos, HashSet<(int x, int y)> cycle)
        {
            int crosses = 0;
            for (int x = 0; x < pos.x; x++)
            {
                if (cycle.Contains((x, pos.y)))
                {
                    switch(grid[(x, pos.y)])
                    {
                        case '|': crosses++; break;
                        case 'L':
                            while (grid[(++x, pos.y)] == '-') { }
                            if (x >= pos.x) break;
                            if (grid[(x, pos.y)] == '7') crosses++; 
                            break;
                        case 'F':
                            while (grid[(++x, pos.y)] == '-') { }
                            if (x >= pos.x) break;
                            if (grid[(x, pos.y)] == 'J') crosses++;
                            break;
                    }
                    
                }
            }
            return crosses;
        }
    }
}
