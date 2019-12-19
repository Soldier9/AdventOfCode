using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day18Solver : AbstractSolver
    {
        class Location
        {
            public readonly Point Position;
            public readonly char Type;

            public int Steps;
            public Location Previous;

            public Location(Location location)
            {
                Position = location.Position;
                Type = location.Type;
                Steps = location.Steps;
            }

            public Location(Point position, char type)
            {
                Position = position;
                Type = type;
            }

            public override bool Equals(object obj)
            {
                return obj is Location location &&
                       EqualityComparer<Point>.Default.Equals(Position, location.Position);
            }

            public override int GetHashCode()
            {
                return -425505606 + Position.GetHashCode();
            }

            public IEnumerable<Point> NeighboorPositions()
            {
                yield return new Point(Position.X - 1, Position.Y);
                yield return new Point(Position.X, Position.Y - 1);
                yield return new Point(Position.X + 1, Position.Y);
                yield return new Point(Position.X, Position.Y + 1);
            }
        }

        Dictionary<Point, Location> Maze = new Dictionary<Point, Location>();
        Dictionary<Tuple<Point, BitArray>, HashSet<Location>> CachedReachableKeys = new Dictionary<Tuple<Point, BitArray>, HashSet<Location>>();

        HashSet<Location> SearchForReachableKeys(Point currentPosition, BitArray collectedKeys, int stepsSoFar)
        {
            var currentState = Tuple.Create(currentPosition, collectedKeys);
            HashSet<Location> reachableKeys;
            if (CachedReachableKeys.ContainsKey(currentState))
            {
                reachableKeys = CachedReachableKeys[currentState];
            }
            else
            {
                reachableKeys = new HashSet<Location>();
                var queue = new Queue<Location>();

                foreach (var location in Maze.Values) location.Steps = int.MaxValue;
                Maze[currentPosition].Steps = 0;
                queue.Enqueue(Maze[currentPosition]);

                while (queue.Count > 0)
                {
                    var currentLocation = queue.Dequeue();

                    if (currentLocation.Type >= 97 && currentLocation.Type <= 122 && !collectedKeys[currentLocation.Type - 97])
                    {
                        reachableKeys.Add(new Location(currentLocation));
                    }

                    foreach (var neighboor in currentLocation.NeighboorPositions())
                    {
                        if (!Maze.ContainsKey(neighboor)
                            || (Maze[neighboor].Type >= 65 && Maze[neighboor].Type <= 90 && !collectedKeys[Maze[neighboor].Type - 65])) continue;

                        if (Maze[neighboor].Steps == int.MaxValue)
                        {
                            Maze[neighboor].Steps = currentLocation.Steps + 1;
                            Maze[neighboor].Previous = currentLocation;
                            queue.Enqueue(Maze[neighboor]);
                        }
                    }
                }

                CachedReachableKeys.Add(currentState, reachableKeys);
            }

            var bestReturnedKeys = new HashSet<Location>();
            foreach (var k in reachableKeys)
            {
                var recursiveCollectedKeys = (BitArray)collectedKeys.Clone();
                recursiveCollectedKeys[k.Type - 97] = true;
                var returnedKeys = SearchForReachableKeys(k.Position, recursiveCollectedKeys, k.Steps + stepsSoFar);
                if (bestReturnedKeys.Count == 0 || returnedKeys.Count > 0 && bestReturnedKeys.Max(l => l.Steps) > returnedKeys.Max(l => l.Steps)) bestReturnedKeys = returnedKeys;
            }

            foreach (var key in reachableKeys)
            {
                var tmp = new Location(key);
                tmp.Steps += stepsSoFar;
                bestReturnedKeys.Add(tmp);
            }

            return bestReturnedKeys;
        }


        public override string Part1()
        {
            Point currentPosition = new Point();
            using (var input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = -1;
                    foreach (var c in input.ReadLine())
                    {
                        x++;
                        if (c == '#') continue;
                        var location = new Location(new Point(x, y), c);
                        Maze.Add(location.Position, location);
                        if (location.Type == '@') currentPosition = location.Position;
                    }
                    y++;
                }
            }

            return SearchForReachableKeys(currentPosition, new BitArray(26), 0).Max(k => k.Steps).ToString();
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
