﻿namespace AdventOfCode.Solvers.Year2020
{
    class Day17Solver : AbstractSolver
    {
        readonly List<(int, int, int)> Neighbors3D = new();
        public List<(int, int, int)> GetNeighbors3D((int, int, int) position)
        {
            return Neighbors3D.Select(n => (n.Item1 + position.Item1, n.Item2 + position.Item2, n.Item3 + position.Item3)).ToList();
        }

        public override string Part1()
        {
            for (int x = -1; x < 2; x++) for (int y = -1; y < 2; y++) for (int z = -1; z < 2; z++) if (!(x == 0 && y == 0 && z == 0)) Neighbors3D.Add((x, y, z));

            HashSet<(int, int, int)> activeNodes = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    foreach (char c in input.ReadLine()!)
                    {
                        if (c == '#') _ = activeNodes.Add((x, y, 0));
                        x++;
                    }
                    y++;
                }
            }

            for (int cycle = 0; cycle < 6; cycle++)
            {
                HashSet<(int, int, int)> nextActiveNodes = new();
                HashSet<(int, int, int)> nodesToCheck = new();

                foreach ((int, int, int) node in activeNodes)
                {
                    foreach ((int, int, int) neighbor in GetNeighbors3D(node))
                    {
                        _ = nodesToCheck.Add(neighbor);
                    }
                }

                foreach ((int, int, int) node in nodesToCheck)
                {
                    int activeNeighbors = 0;
                    foreach ((int, int, int) neighbor in GetNeighbors3D(node))
                    {
                        if (activeNodes.Contains(neighbor)) activeNeighbors++;
                    }

                    bool newState = false;
                    if (activeNodes.Contains(node)) newState = (activeNeighbors >= 2 && activeNeighbors <= 3);
                    else newState = (activeNeighbors == 3);

                    if (newState) _ = nextActiveNodes.Add(node);
                }
                activeNodes = nextActiveNodes;
            }

            return activeNodes.Count.ToString();
        }

        readonly List<(int, int, int, int)> Neighbors4D = new();
        public List<(int, int, int, int)> GetNeighbors4D((int, int, int, int) position)
        {
            return Neighbors4D.Select(n => (n.Item1 + position.Item1, n.Item2 + position.Item2, n.Item3 + position.Item3, n.Item4 + position.Item4)).ToList();
        }

        public override string Part2()
        {
            for (int x = -1; x < 2; x++) for (int y = -1; y < 2; y++) for (int z = -1; z < 2; z++) for (int w = -1; w < 2; w++) if (!(x == 0 && y == 0 && z == 0 && w == 0)) Neighbors4D.Add((x, y, z, w));

            HashSet<(int, int, int, int)> activeNodes = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    foreach (char c in input.ReadLine()!)
                    {
                        if (c == '#') _ = activeNodes.Add((x, y, 0, 0));
                        x++;
                    }
                    y++;
                }
            }

            for (int cycle = 0; cycle < 6; cycle++)
            {
                HashSet<(int, int, int, int)> nextActiveNodes = new();
                HashSet<(int, int, int, int)> nodesToCheck = new();

                foreach ((int, int, int, int) node in activeNodes)
                {
                    foreach ((int, int, int, int) neighbor in GetNeighbors4D(node))
                    {
                        _ = nodesToCheck.Add(neighbor);
                    }
                }

                foreach ((int, int, int, int) node in nodesToCheck)
                {
                    int activeNeighbors = 0;
                    foreach ((int, int, int, int) neighbor in GetNeighbors4D(node))
                    {
                        if (activeNodes.Contains(neighbor)) activeNeighbors++;
                    }

                    bool newState = false;
                    if (activeNodes.Contains(node)) newState = (activeNeighbors >= 2 && activeNeighbors <= 3);
                    else newState = (activeNeighbors == 3);

                    if (newState) _ = nextActiveNodes.Add(node);
                }
                activeNodes = nextActiveNodes;
            }

            return activeNodes.Count.ToString();
        }
    }
}
