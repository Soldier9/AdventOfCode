using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Solvers
{
    class Day20Solver : AbstractSolver
    {
        public class Portal
        {
            public Point entry;
            public Point exit;

            public Portal(Point entry)
            {
                this.entry = entry;
            }
        }

        class Location
        {
            public readonly Point Position;
            public char Type;
            public readonly List<Location> Neighboors = new List<Location>();

            public int Steps = int.MaxValue;
            public int Level = 0;
            public Location(Point position, char type, int level)
            {
                Position = position;
                Type = type;
                Level = level;
            }

            public void LinkNeighboors(Dictionary<Point, Location> maze)
            {
                var neighboorLocation = new Point(Position.X - 1, Position.Y);
                if (maze.ContainsKey(neighboorLocation)) Neighboors.Add(maze[neighboorLocation]);
                neighboorLocation = new Point(Position.X + 1, Position.Y);
                if (maze.ContainsKey(neighboorLocation)) Neighboors.Add(maze[neighboorLocation]);
                neighboorLocation = new Point(Position.X, Position.Y - 1);
                if (maze.ContainsKey(neighboorLocation)) Neighboors.Add(maze[neighboorLocation]);
                neighboorLocation = new Point(Position.X, Position.Y + 1);
                if (maze.ContainsKey(neighboorLocation)) Neighboors.Add(maze[neighboorLocation]);
            }
        }


        Dictionary<Point, Location> CreateMaze(List<string> input, int level = 0)
        {
            Dictionary<Point, Location> maze = new Dictionary<Point, Location>();
            var portals = new Dictionary<string, Portal>();
            for (var y = 0; y < input.Count; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    var currentChar = input[y][x];
                    switch (currentChar)
                    {
                        case ' ': continue;
                        case '#': continue;
                        case '.':
                            var position = new Point(x, y);
                            maze.Add(position, new Location(position, currentChar, level));
                            break;
                        default:
                            if (y > 0 && y < input.Count - 1)
                            {
                                if (input[y - 1][x] > 64 && input[y + 1][x] == '.')
                                {
                                    var portalName = new string(new char[] { input[y - 1][x], input[y][x] });
                                    if (portals.ContainsKey(portalName)) portals[portalName].exit = new Point(x, y + 1);
                                    else portals.Add(portalName, new Portal(new Point(x, y + 1)));
                                }
                                else if (input[y + 1][x] > 64 && input[y - 1][x] == '.')
                                {
                                    var portalName = new string(new char[] { input[y][x], input[y + 1][x] });
                                    if (portals.ContainsKey(portalName)) portals[portalName].exit = new Point(x, y - 1);
                                    else portals.Add(portalName, new Portal(new Point(x, y - 1)));
                                }
                            }
                            if (x > 0 && x < input[0].Length - 1)
                            {
                                if (input[y][x - 1] > 64 && input[y][x + 1] == '.')
                                {
                                    var portalName = new string(new char[] { input[y][x - 1], input[y][x] });
                                    if (portals.ContainsKey(portalName)) portals[portalName].exit = new Point(x + 1, y);
                                    else portals.Add(portalName, new Portal(new Point(x + 1, y)));
                                }
                                else if (input[y][x + 1] > 64 && input[y][x - 1] == '.')
                                {
                                    var portalName = new string(new char[] { input[y][x], input[y][x + 1] });
                                    if (portals.ContainsKey(portalName)) portals[portalName].exit = new Point(x - 1, y);
                                    else portals.Add(portalName, new Portal(new Point(x - 1, y)));
                                }
                            }
                            break;
                    }
                }
            }

            foreach (var location in maze.Values) location.LinkNeighboors(maze);
            foreach (var portal in portals)
            {
                if (portal.Key == "AA") maze[portal.Value.entry].Type = 'I';
                else if (portal.Key == "ZZ") maze[portal.Value.entry].Type = 'O';
                else
                {
                    maze[portal.Value.entry].Neighboors.Add(maze[portal.Value.exit]);
                    maze[portal.Value.exit].Neighboors.Add(maze[portal.Value.entry]);
                }
            }
            return maze;
        }


        public override string Part1()
        {
            List<string> theInput = new List<string>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream) theInput.Add(input.ReadLine());
            }

            Dictionary<Point, Location> maze = CreateMaze(theInput);

            Queue<Location> queue = new Queue<Location>();
            var entry = maze.Single(l => l.Value.Type == 'I').Value;
            entry.Steps = 0;
            queue.Enqueue(entry);
            while (queue.Count > 0)
            {
                var currentLocation = queue.Dequeue();
                if (currentLocation.Type == 'O') return currentLocation.Steps.ToString();
                foreach (var neighboor in currentLocation.Neighboors)
                {
                    if (neighboor.Steps > currentLocation.Steps + 1)
                    {
                        neighboor.Steps = currentLocation.Steps + 1;
                        queue.Enqueue(neighboor);
                    }
                }
            }

            return "No solution found!";
        }

        public override string Part2()
        {
            List<string> theInput = new List<string>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream) theInput.Add(input.ReadLine());
            }

            List<Dictionary<Point, Location>> mazes = new List<Dictionary<Point, Location>>
            {
                CreateMaze(theInput)
            };

            Queue<Location> queue = new Queue<Location>();
            var entry = mazes[0].Single(l => l.Value.Type == 'I').Value;
            entry.Steps = 0;
            queue.Enqueue(entry);

            while (queue.Count > 0)
            {
                var currentLocation = queue.Dequeue();
                if (currentLocation.Type == 'O' && currentLocation.Level == 0) return currentLocation.Steps.ToString();
                foreach (var neighboor in currentLocation.Neighboors)
                {
                    Location actualNeighboor;
                    if (((neighboor.Position.X == 2 || neighboor.Position.X == 114) && Math.Abs(neighboor.Position.X - currentLocation.Position.X) > 1) || ((neighboor.Position.Y == 2 || neighboor.Position.Y == 114) && Math.Abs(neighboor.Position.Y - currentLocation.Position.Y) > 1))
                    {
                        var newLevel = currentLocation.Level + 1;
                        if (mazes.Count == newLevel) mazes.Add(CreateMaze(theInput, newLevel));
                        actualNeighboor = mazes[newLevel][neighboor.Position];
                    }
                    else if (((neighboor.Position.X == 28 || neighboor.Position.X == 88) && Math.Abs(neighboor.Position.X - currentLocation.Position.X) > 1) || ((neighboor.Position.Y == 28 || neighboor.Position.Y == 88) && Math.Abs(neighboor.Position.Y - currentLocation.Position.Y) > 1))
                    {
                        if (currentLocation.Level == 0) continue;
                        var newLevel = currentLocation.Level - 1;
                        actualNeighboor = mazes[newLevel][neighboor.Position];
                    }
                    else
                    {
                        if (neighboor.Type == 'I') continue;
                        if (neighboor.Type == 'O' && currentLocation.Level > 0) continue;
                        actualNeighboor = neighboor;
                    }

                    if (actualNeighboor.Steps > currentLocation.Steps + 1)
                    {
                        actualNeighboor.Steps = currentLocation.Steps + 1;
                        queue.Enqueue(actualNeighboor);
                    }
                }
            }

            return "No solution found!";
        }

    }
}
