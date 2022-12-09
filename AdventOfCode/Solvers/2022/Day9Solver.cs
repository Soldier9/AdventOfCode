using System.Security.Principal;
using System.Text;

namespace AdventOfCode.Solvers.Year2022
{
    class Day9Solver : AbstractSolver
    {
        public override bool HasVisualization => true;

        public override string Part1()
        {
            HashSet<(int, int)> visited = new();
            (int x, int y) head = (0, 0);
            (int x, int y) tail = (0, 0);
            visited.Add(tail);

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(" ");
                    for (int i = 0; i < int.Parse(line[1]); i++)
                    {
                        switch (line[0])
                        {
                            case "U": head.y++; break;
                            case "D": head.y--; break;
                            case "L": head.x--; break;
                            case "R": head.x++; break;
                        }

                        if (!IsTouching(head, tail)) tail = MoveTail(head, tail);
                        visited.Add(tail);
                    }
                }
            }

            return visited.Count.ToString();
        }

        private (int x, int y) MoveTail((int x, int y) head, (int x, int y) tail)
        {
            if (head.x == tail.x)
            {
                if (head.y > tail.y) tail.y++;
                else tail.y--;
            }
            else if (head.y == tail.y)
            {
                if (head.x > tail.x) tail.x++;
                else tail.x--;
            }
            else if (head.x > tail.x && head.y > tail.y)
            {
                tail.x++; tail.y++;
            }
            else if (head.x > tail.x && head.y < tail.y)
            {
                tail.x++; tail.y--;
            }
            else if (head.x < tail.x && head.y > tail.y)
            {
                tail.x--; tail.y++;
            }
            else if (head.x < tail.x && head.y < tail.y)
            {
                tail.x--; tail.y--;
            }
            return tail;
        }

        private bool IsTouching((int x, int y) head, (int x, int y) tail)
        {
            int xDist = Math.Abs(head.x - tail.x);
            int yDist = Math.Abs(head.y - tail.y);
            return Math.Max(yDist, xDist) < 2;
        }

        public override string Part2()
        {
            HashSet<(int, int)> visited = new();
            List<(int x, int y)> rope = new();
            for (int i = 0; i < 10; i++) rope.Add((0, 0));
            visited.Add(rope.Last());

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(" ");
                    for (int i = 0; i < int.Parse(line[1]); i++)
                    {
                        switch (line[0])
                        {
                            case "U": rope[0] = (rope[0].x, rope[0].y + 1); break;
                            case "D": rope[0] = (rope[0].x, rope[0].y - 1); break;
                            case "L": rope[0] = (rope[0].x - 1, rope[0].y); break;
                            case "R": rope[0] = (rope[0].x + 1, rope[0].y); break;
                        }

                        for (int j = 1; j < rope.Count; j++) if (!IsTouching(rope[j - 1], rope[j])) rope[j] = MoveTail(rope[j - 1], rope[j]);
                        visited.Add(rope.Last());
                        if (Program.VisualizationEnabled) PrintRope(rope);
                    }
                }
            }

            return visited.Count.ToString();
        }

        private (int x, int y) allTimeMin = (0, 0);
        private (int x, int y) allTimeMax = (0, 0);
        private void PrintRope(List<(int x, int y)> rope)
        {
            Dictionary<(int x, int y), string> hashedRope = new();
            for (int i = 0; i < rope.Count; i++)
            {
                char rep = '#';
                if (i == 0) rep = 'H';
                if (i == rope.Count - 1) rep = 'T';
                hashedRope.TryAdd(rope[i], "\u001b[38;5;" + (255 - i).ToString() + "m" + rep + "\u001b[0m");
            }

            (int x, int y) min = (rope.MinBy(r => r.x).x, rope.MinBy(r => r.y).y);
            (int x, int y) max = (rope.MaxBy(r => r.x).x, rope.MaxBy(r => r.y).y);
            allTimeMin.x = Math.Min(allTimeMin.x, min.x);
            allTimeMin.y = Math.Min(allTimeMin.y, min.y);
            allTimeMax.x = Math.Max(allTimeMax.x, max.x);
            allTimeMax.y = Math.Max(allTimeMax.y, max.y);

            StringBuilder sb = new();
            for (int y = allTimeMin.y; y <= allTimeMax.y; y++)
            {
                StringBuilder nextLine = new();
                for (int x = allTimeMin.x; x <= allTimeMax.x; x++)
                {
                    if (hashedRope.ContainsKey((x, y))) nextLine.Append(hashedRope[(x, y)]);
                    else if (x == 0 && y == 0) nextLine.Append("s");
                    else nextLine.Append(" ");
                }
                sb.AppendLine(nextLine.ToString());
            }
            Program.PrintData(sb.ToString(), 0, false, true);
        }
    }
}
