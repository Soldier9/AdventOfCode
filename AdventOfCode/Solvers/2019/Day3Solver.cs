namespace AdventOfCode.Solvers.Year2019
{
    class Day3Solver : AbstractSolver
    {
        class Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Point Clone()
            {
                return new Point(X, Y);
            }

            public override bool Equals(object? obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return X == ((Point)obj).X && Y == ((Point)obj).Y;
            }

            public override string ToString()
            {
                return "(" + X + ", " + Y + ")";
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }

        static Dictionary<Point, int> TraceWire(string[] moves)
        {
            Dictionary<Point, int> tracedWire = new();

            Point currentPoint = new(0, 0);
            int wireLength = 0;

            foreach (string movement in moves)
            {
                int moveLength = int.Parse(movement[1..]);
                for (int i = 0; i < moveLength; i++)
                {
                    switch (movement[0])
                    {
                        case 'R': currentPoint.X++; break;
                        case 'L': currentPoint.X--; break;
                        case 'U': currentPoint.Y++; break;
                        case 'D': currentPoint.Y--; break;
                    }

                    wireLength++;
                    if (!tracedWire.ContainsKey(currentPoint)) tracedWire.Add(currentPoint.Clone(), wireLength);
                }
            }

            return tracedWire;
        }

        public override string Part1()
        {
            Dictionary<Point, int> wire1;
            Dictionary<Point, int> wire2;

            using (StreamReader input = File.OpenText(InputFile))
            {
                wire1 = TraceWire(input.ReadLine()!.Split(',').ToArray());
                wire2 = TraceWire(input.ReadLine()!.Split(',').ToArray());
            }

            IEnumerable<KeyValuePair<Point, int>> intersects = wire1.Where(p => wire2.ContainsKey(p.Key)).Select(p => new KeyValuePair<Point, int>(p.Key, p.Value + wire2[p.Key]));

            return intersects.Min(p => Math.Abs(p.Key.X) + Math.Abs(p.Key.Y)).ToString();
        }

        public override string Part2()
        {
            Dictionary<Point, int> wire1;
            Dictionary<Point, int> wire2;

            using (StreamReader input = File.OpenText(InputFile))
            {
                wire1 = TraceWire(input.ReadLine()!.Split(',').ToArray());
                wire2 = TraceWire(input.ReadLine()!.Split(',').ToArray());
            }

            IEnumerable<KeyValuePair<Point, int>> intersects = wire1.Where(p => wire2.ContainsKey(p.Key)).Select(p => new KeyValuePair<Point, int>(p.Key, p.Value + wire2[p.Key]));

            int shortestDistance = int.MaxValue;
            foreach (KeyValuePair<Point, int> intersect in intersects)
            {
                if (intersect.Value < shortestDistance) shortestDistance = intersect.Value;
            }

            return shortestDistance.ToString();
        }
    }
}
