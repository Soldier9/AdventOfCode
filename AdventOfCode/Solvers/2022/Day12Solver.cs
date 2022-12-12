
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode.Solvers.Year2022
{
    class Day12Solver : AbstractSolver
    {
        public override bool HasVisualization => true;

        Dictionary<(int x, int y), char> map = new();
        int height, width;
        (int x, int y) start = (-1, -1);
        (int x, int y) dest = (-1, -1);

        public override string Part1()
        {
            Dictionary<(int x, int y), int> costs = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    foreach (char c in input.ReadLine()!)
                    {
                        costs.Add((x, y), int.MaxValue);
                        if (c == 'S')
                        {
                            start = (x, y);
                            map.Add((x, y), 'a');
                        }
                        else if (c == 'E')
                        {
                            dest = (x, y);
                            map.Add((x, y), 'z');
                        }
                        else map.Add((x, y), c);

                        x++;
                        width = Math.Max(width, x);
                    }
                    y++;
                    height = Math.Max(height, y);
                }
            }

            costs[start] = 0;
            HashSet<(int x, int y)> dests = new();
            dests.Add(dest);
            VisitPos(start, dests, costs, new());

            return costs[dest].ToString();
        }
        private bool VisitPos((int x, int y) pos, HashSet<(int x, int y)> dests, Dictionary<(int x, int y), int> costs, PriorityQueue<(int x, int y), int> queue, bool part2 = false)
        {
            List<(int x, int y)> neighbors = new();
            if (pos.y > 0) neighbors.Add((pos.x, pos.y - 1));
            if (pos.y < height - 1) neighbors.Add((pos.x, pos.y + 1));
            if (pos.x > 0) neighbors.Add((pos.x - 1, pos.y));
            if (pos.x < width - 1) neighbors.Add((pos.x + 1, pos.y));

            foreach ((int x, int y) neighbor in neighbors)
            {
                if ((!part2 && map[pos] >= map[neighbor] - 1 && costs[neighbor] > costs[pos] + 1)
                    || (part2 && map[pos] <= map[neighbor] + 1 && costs[neighbor] > costs[pos] + 1))
                {
                    costs[neighbor] = costs[pos] + 1;
                    queue.Enqueue(neighbor, costs[neighbor]);
                    if (dests.Contains(neighbor)) return true;
                }
            }

            if (Program.VisualizationEnabled) Print(costs);
            while (queue.Count > 0)
            {
                if (VisitPos(queue.Dequeue(), dests, costs, queue, part2)) return true;
            }
            return false;
        }

        private void Print(Dictionary<(int x, int y), int> costs)
        {
            StringBuilder sb = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    if (costs[(x, y)] < int.MaxValue) _ = sb.Append(costs[(x, y)].ToString()[^1]);
                    else _ = sb.Append(map[(x, y)]);
                }
                _ = sb.Append("\r\n");
            }
            Program.PrintData(sb.ToString(), 0, true);
        }

        public override string Part2()
        {
            Dictionary<(int x, int y), int> costs = new();
            HashSet<(int x, int y)> dests = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[(x, y)] == 'a') dests.Add((x, y));
                    costs.Add((x, y), int.MaxValue);
                }
            }
            costs[dest] = 0;

            VisitPos(dest, dests, costs, new(), true);

            return costs[dests.Single(d => costs[d] < int.MaxValue)].ToString();
        }
    }
}
