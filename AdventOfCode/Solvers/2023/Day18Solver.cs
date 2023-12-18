namespace AdventOfCode.Solvers.Year2023
{
    class Day18Solver : AbstractSolver
    {
        List<(string direction, int length, string color)> instructions = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ');
                    instructions.Add((line[0], int.Parse(line[1]), line[2][2..8]));
                }
            }

            HashSet<(int x, int y)> hole = new();
            (int x, int y) location = (0, 0);

            foreach ((string direction, int length, string color) instruction in instructions)
            {
                for (int i = 0; i < instruction.length; i++)
                {
                    switch (instruction.direction)
                    {
                        case "U": location.y--; break;
                        case "D": location.y++; break;
                        case "L": location.x--; break;
                        case "R": location.x++; break;
                    }
                    hole.Add(location);
                }
            }

            hole = Floodfill(hole);
            return hole.Count.ToString();
        }

        static HashSet<(int x, int y)> Floodfill(HashSet<(int x, int y)> hole)
        {

            (int x, int y) min = (hole.Min(h => h.x) - 1, hole.Min(h => h.y) - 1);
            (int x, int y) max = (hole.Max(h => h.x) + 1, hole.Max(h => h.y) + 1);

            HashSet<(int x, int y)> nonHole = hole.ToHashSet();
            Queue<(int x, int y)> queue = new();
            queue.Enqueue(min);

            while (queue.Count > 0)
            {
                (int x, int y) position = queue.Dequeue();
                HashSet<(int x, int y)> neighbors = new();
                if (!nonHole.Contains((position.x - 1, position.y))) neighbors.Add((position.x - 1, position.y));
                if (!nonHole.Contains((position.x + 1, position.y))) neighbors.Add((position.x + 1, position.y));
                if (!nonHole.Contains((position.x, position.y - 1))) neighbors.Add((position.x, position.y - 1));
                if (!nonHole.Contains((position.x, position.y + 1))) neighbors.Add((position.x, position.y + 1));
                neighbors.RemoveWhere(n => n.x < min.x || n.x > max.x || n.y < min.y || n.y > max.y);

                foreach ((int x, int y) neighbor in neighbors)
                {
                    if (nonHole.Add(neighbor)) queue.Enqueue(neighbor);
                }
            }

            HashSet<(int x, int y)> filled = hole.ToHashSet();
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    if (!nonHole.Contains((x, y))) filled.Add((x, y));
                }
            }
            return filled;
        }

        public override string Part2()
        {
            List<(long x, long y)> hole = new();
            long result = 0;

            (long x, long y) position = (0, 0);
            foreach ((string direction, int length, string color) instruction in instructions)
            {
                int distance = Convert.ToInt32(instruction.color[..5], 16);
                switch (instruction.color.Last())
                {
                    case '0': position.x += distance; break;
                    case '1': position.y += distance; break;
                    case '2': position.x -= distance; break;
                    case '3': position.y -= distance; break;
                }
                hole.Add(position);
                result += distance;
            }

            result += 2;
            result /= 2;

            result += Shoelace(hole);
            return result.ToString();
        }

        static long Shoelace(List<(long x, long y)> hole)
        {
            long result = 0;

            for (int i = 0; i < hole.Count - 1; i++)
            {
                result += (hole[i].x * hole[i + 1].y);
                result -= (hole[i].y * hole[i + 1].x);
            }
            result += (hole[hole.Count - 1].x * hole[0].y);
            result -= (hole[hole.Count - 1].y * hole[0].x);

            return result / 2;
        }
    }
}
