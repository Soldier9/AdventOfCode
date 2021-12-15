namespace AdventOfCode.Solvers.Year2021
{
    class Day15Solver : AbstractSolver
    {
        class Location
        {
            static public Dictionary<(int, int), Location> Grid = new();
            static public int MaxX = 0;
            static public int MaxY = 0;

            public int Risk;
            public int TotalRisk;

            (int x, int y) Position;

            public Location(int risk, int x, int y)
            {
                Risk = risk;
                while (Risk > 9) Risk -= 9;

                Position = (x, y);

                if (Position.x == 0 && Position.y == 0) TotalRisk = 0;
                else TotalRisk = int.MaxValue;

                MaxX = Math.Max(MaxX, Position.x);
                MaxY = Math.Max(MaxY, Position.y);
            }

            public int FindCostTo((int x, int y) destination)
            {
                PriorityQueue<Location, int> priorityQueue = new();
                priorityQueue.Enqueue(this, TotalRisk);

                while (priorityQueue.Peek().TotalRisk < Grid[destination].TotalRisk)
                {
                    Location location = priorityQueue.Dequeue();
                    foreach (Location neighbor in location.GetNeighbors())
                    {
                        if (neighbor.TotalRisk > location.TotalRisk + neighbor.Risk)
                        {
                            neighbor.TotalRisk = location.TotalRisk + neighbor.Risk;
                            priorityQueue.Enqueue(neighbor, neighbor.TotalRisk);
                        }
                    }
                }

                return Grid[destination].TotalRisk;
            }

            List<Location> GetNeighbors()
            {
                List<Location> neighbors = new();
                if (Position.x > 0) neighbors.Add(Grid[(Position.x - 1, Position.y)]);
                if (Position.x < MaxX) neighbors.Add(Grid[(Position.x + 1, Position.y)]);

                if (Position.y > 0) neighbors.Add(Grid[(Position.x, Position.y - 1)]);
                if (Position.y < MaxY) neighbors.Add(Grid[(Position.x, Position.y + 1)]);

                return neighbors;
            }
        }

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
                        Location.Grid.Add((x, y), new Location(int.Parse(line[x].ToString()), x, y));
                    }
                    y++;
                }
            }

            return Location.Grid[(0, 0)].FindCostTo((Location.MaxX, Location.MaxY)).ToString();
        }

        public override string Part2()
        {

            Dictionary<(int, int), Location> orgGrid = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;

                    for (int x = 0; x < line.Length; x++)
                    {
                        orgGrid.Add((x, y), new Location(int.Parse(line[x].ToString()), x, y));
                    }
                    y++;
                }
            }

            Location.Grid = new Dictionary<(int, int), Location>();
            int orgWidth = Location.MaxX + 1;
            int orgHeight = Location.MaxY + 1;
            for (int x = 0; x < orgWidth; x++)
            {
                for (int y = 0; y < orgHeight; y++)
                {
                    for (int nx = 0; nx < 5; nx++)
                    {
                        for (int ny = 0; ny < 5; ny++)
                        {
                            (int x, int y) position = (x + (nx * orgWidth), y + (ny * orgHeight));
                            Location.Grid.Add(position, new Location(orgGrid[(x, y)].Risk + nx + ny, position.x, position.y));
                        }
                    }
                }
            }

            return Location.Grid[(0, 0)].FindCostTo((Location.MaxX, Location.MaxY)).ToString();
        }
    }
}
