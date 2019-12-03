using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
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

            public override bool Equals(object obj)
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
                var hashCode = 1861411795;
                hashCode = hashCode * -1521134295 + X.GetHashCode();
                hashCode = hashCode * -1521134295 + Y.GetHashCode();
                return hashCode;
            }
        }

        void TraceWire(string[] moves, HashSet<Point> points)
        {
            Point currentPoint = new Point(0, 0);

            foreach(string movement in moves)
            {
                int moveLength = int.Parse(movement.Substring(1));
                for (int i = 0; i < moveLength; i++)
                {
                    switch (movement[0])
                    {
                        case 'R': currentPoint.X++; break;
                        case 'L': currentPoint.X--; break;
                        case 'U': currentPoint.Y++; break;
                        case 'D': currentPoint.Y--; break;
                    }

                    points.Add(currentPoint.Clone());
                }
            }
        }

        public override string Part1()
        {
            HashSet<Point> wire1 = new HashSet<Point>();
            HashSet<Point> wire2 = new HashSet<Point>();

            using (var input = File.OpenText(InputFile))
            {
                TraceWire(input.ReadLine().Split(',').ToArray(), wire1);
                TraceWire(input.ReadLine().Split(',').ToArray(), wire2);
            }

            List<Point> intersects = wire1.Where(p => wire2.Contains(p)).ToList();
            int shortestDistance = Int32.MaxValue;
            foreach(Point point in intersects)
            {
                var distanceToZero = Math.Abs(point.X) + Math.Abs(point.Y);
                if (distanceToZero < shortestDistance) shortestDistance = distanceToZero;
            }

            return shortestDistance.ToString();
        }

        int GetDistAlongWire(string[] moves, Point destPoint)
        {
            Point currentPoint = new Point(0, 0);
            
            int distMoved = 0;
            foreach (string movement in moves)
            {
                int moveLength = int.Parse(movement.Substring(1));
                for (int i = 0; i < moveLength; i++)
                {
                    switch (movement[0])
                    {
                        case 'R': currentPoint.X++; break;
                        case 'L': currentPoint.X--; break;
                        case 'U': currentPoint.Y++; break;
                        case 'D': currentPoint.Y--; break;
                    }

                    distMoved++;
                    if (currentPoint.Equals(destPoint)) return distMoved;
                }
            }

            return 0;
        }

        public override string Part2()
        {
            string[] wire1moves;
            string[] wire2moves;

            HashSet<Point> wire1 = new HashSet<Point>();
            HashSet<Point> wire2 = new HashSet<Point>();

            using (var input = File.OpenText(InputFile))
            {
                wire1moves = input.ReadLine().Split(',').ToArray();
                wire2moves = input.ReadLine().Split(',').ToArray();

                TraceWire(wire1moves, wire1);
                TraceWire(wire2moves, wire2);
            }

            List<Point> intersects = wire1.Where(p => wire2.Contains(p)).ToList();

            int shortestDist = Int32.MaxValue;
            foreach(Point intersect in intersects)
            {
                int thisDist = GetDistAlongWire(wire1moves, intersect) + GetDistAlongWire(wire2moves, intersect);
                if (thisDist < shortestDist) shortestDist = thisDist;
            }

            return shortestDist.ToString();
        }

        

    }
}
