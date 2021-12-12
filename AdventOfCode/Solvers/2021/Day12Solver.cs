using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day12Solver : AbstractSolver
    {
        class Cave
        {
            public static HashSet<Cave> Caves = new HashSet<Cave>();

            public string Name;
            bool BigCave;
            HashSet<Cave> Neighbors = new HashSet<Cave>();

            public static void AddCaves(string link)
            {
                string[] linkedCaves = link.Split('-');
                Cave[] caves = new Cave[2];
                if (Caves.Where(c => c.Name == linkedCaves[0]).Count() == 0)
                {
                    Cave newCave = new Cave()
                    {
                        Name = linkedCaves[0],
                        BigCave = Char.IsUpper(linkedCaves[0][0])
                    };
                    caves[0] = newCave;
                    Cave.Caves.Add(caves[0]);
                }
                else caves[0] = Caves.Single(c => c.Name == linkedCaves[0]);

                if (Caves.Where(c => c.Name == linkedCaves[1]).Count() == 0)
                {
                    Cave newCave = new Cave()
                    {
                        Name = linkedCaves[1],
                        BigCave = Char.IsUpper(linkedCaves[1][0])
                    };
                    caves[1] = newCave;
                    Cave.Caves.Add(caves[1]);
                }
                else caves[1] = Caves.Single(c => c.Name == linkedCaves[1]);

                caves[0].Neighbors.Add(caves[1]);
                caves[1].Neighbors.Add(caves[0]);
            }

            public List<List<Cave>> PathsToEnd(List<Cave> visited)
            {
                List<List<Cave>> paths = new List<List<Cave>>();
                if (Name == "end")
                {
                    List<Cave> path = new List<Cave> { this };
                    paths.Add(path);
                    return paths;
                }

                visited.Add(this);
                foreach (var neighbor in Neighbors.Where(c => c.BigCave || !visited.Contains(c)))
                {
                    foreach (List<Cave> neighborPath in neighbor.PathsToEnd(new List<Cave>(visited)))
                    {
                        List<Cave> path = new List<Cave> { this };
                        path.AddRange(neighborPath);
                        paths.Add(path);
                    }
                }
                return paths;
            }

            public List<List<Cave>> PathsToEndPart2(List<Cave> visited, bool doubleVisitUsed)
            {
                List<List<Cave>> paths = new List<List<Cave>>();
                if (Name == "end")
                {
                    List<Cave> path = new List<Cave> { this };
                    paths.Add(path);
                    return paths;
                }

                visited.Add(this);
                foreach (var neighbor in Neighbors.Where(c => !doubleVisitUsed || c.BigCave || !visited.Contains(c)))
                {
                    if (neighbor.Name == "start") continue;
                    foreach (List<Cave> neighborPath in neighbor.PathsToEndPart2(new List<Cave>(visited), (doubleVisitUsed || (!neighbor.BigCave && visited.Contains(neighbor)))))
                    {
                        List<Cave> path = new List<Cave> { this };
                        path.AddRange(neighborPath);
                        paths.Add(path);
                    }
                }
                return paths;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Cave.AddCaves(input.ReadLine());
                }
            }

            return Cave.Caves.Single(c => c.Name == "start").PathsToEnd(new List<Cave>()).Count().ToString();
        }

        public override string Part2()
        {
            return Cave.Caves.Single(c => c.Name == "start").PathsToEndPart2(new List<Cave>(), false).Count().ToString();
        }
    }
}