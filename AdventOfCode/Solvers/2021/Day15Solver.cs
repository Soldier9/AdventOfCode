﻿using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day15Solver : AbstractSolver
    {
        public override bool HasVisualization => true;
        public override bool HasExtendedVisualization => true;

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
                    if (Program.ExtendedVisualization) PrintState(location, true);
                    foreach (Location neighbor in location.GetNeighbors())
                    {
                        if (neighbor.TotalRisk > location.TotalRisk + neighbor.Risk)
                        {
                            neighbor.TotalRisk = location.TotalRisk + neighbor.Risk;
                            priorityQueue.Enqueue(neighbor, neighbor.TotalRisk);
                        }
                    }
                }
                if (Program.VisualizationEnabled) PrintState(Grid[destination]);
                return Grid[destination].TotalRisk;
            }

            private List<Location> GetNeighbors()
            {
                List<Location> neighbors = new();
                if (Position.x > 0) neighbors.Add(Grid[(Position.x - 1, Position.y)]);
                if (Position.x < MaxX) neighbors.Add(Grid[(Position.x + 1, Position.y)]);

                if (Position.y > 0) neighbors.Add(Grid[(Position.x, Position.y - 1)]);
                if (Position.y < MaxY) neighbors.Add(Grid[(Position.x, Position.y + 1)]);

                return neighbors;
            }

            private static void PrintState(Location bestPathSoFar, bool cropOutput = false)
            {
                HashSet<Location> bestPath = new();
                Location backtrackLocation = bestPathSoFar;
                _ = bestPath.Add(backtrackLocation);
                while (backtrackLocation.Position != (0, 0))
                {
                    backtrackLocation = backtrackLocation.GetNeighbors().MinBy(l => l.TotalRisk)!;
                    _ = bestPath.Add(backtrackLocation);
                }

                StringBuilder sb = new();
                for (int y = 0; y <= MaxY; y++)
                {
                    for (int x = 0; x <= MaxX; x++)
                    {
                        if (bestPath.Contains(Grid[(x, y)]))
                        {
                            _ = sb.Append("\u001b[48;5;8m");
                            _ = sb.Append("\u001b[38;5;9m");
                            _ = sb.Append(Grid[(x, y)].Risk);
                            _ = sb.Append("\u001b[0m");
                        }
                        else
                        {
                            if (Grid[(x, y)].TotalRisk != int.MaxValue) _ = sb.Append("\u001b[48;5;8m");
                            _ = sb.Append(Grid[(x, y)].Risk);
                            if (Grid[(x, y)].TotalRisk != int.MaxValue) _ = sb.Append("\u001b[0m");
                        }
                    }
                    _ = sb.Append("\r\n");
                }
                for (int x = 0; x <= MaxX; x++) _ = sb.Append(' ');
                Program.PrintData(sb.ToString(), 0, false, cropOutput);
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
