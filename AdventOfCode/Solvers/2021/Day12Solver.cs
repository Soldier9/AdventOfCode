namespace AdventOfCode.Solvers.Year2021
{
    class Day12Solver : AbstractSolver
    {
        class Cave
        {
            public static HashSet<Cave> Caves = new();

            public string? Name;
            bool BigCave;
            readonly HashSet<Cave> Neighbors = new();

            public static void AddCaves(string link)
            {
                string[] linkedCaves = link.Split('-');
                Cave[] caves = new Cave[2];
                if (!Caves.Any(c => c.Name == linkedCaves[0]))
                {
                    Cave newCave = new()
                    {
                        Name = linkedCaves[0],
                        BigCave = char.IsUpper(linkedCaves[0][0])
                    };
                    caves[0] = newCave;
                    _ = Cave.Caves.Add(caves[0]);
                }
                else
                {
                    caves[0] = Caves.Single(c => c.Name == linkedCaves[0]);
                }

                if (!Caves.Any(c => c.Name == linkedCaves[1]))
                {
                    Cave newCave = new()
                    {
                        Name = linkedCaves[1],
                        BigCave = char.IsUpper(linkedCaves[1][0])
                    };
                    caves[1] = newCave;
                    _ = Cave.Caves.Add(caves[1]);
                }
                else
                {
                    caves[1] = Caves.Single(c => c.Name == linkedCaves[1]);
                }

                _ = caves[0].Neighbors.Add(caves[1]);
                _ = caves[1].Neighbors.Add(caves[0]);
            }

            public List<List<Cave>> PathsToEnd(List<Cave> visited)
            {
                List<List<Cave>> paths = new();
                if (Name == "end")
                {
                    List<Cave> path = new() { this };
                    paths.Add(path);
                    return paths;
                }

                visited.Add(this);
                foreach (Cave neighbor in Neighbors.Where(c => c.BigCave || !visited.Contains(c)))
                {
                    foreach (List<Cave> neighborPath in neighbor.PathsToEnd(new List<Cave>(visited)))
                    {
                        List<Cave> path = new() { this };
                        path.AddRange(neighborPath);
                        paths.Add(path);
                    }
                }
                return paths;
            }

            public List<List<Cave>> PathsToEndPart2(List<Cave> visited, bool doubleVisitUsed)
            {
                List<List<Cave>> paths = new();
                if (Name == "end")
                {
                    List<Cave> path = new() { this };
                    paths.Add(path);
                    return paths;
                }

                visited.Add(this);
                foreach (Cave neighbor in Neighbors.Where(c => !doubleVisitUsed || c.BigCave || !visited.Contains(c)))
                {
                    if (neighbor.Name == "start") continue;
                    foreach (List<Cave> neighborPath in neighbor.PathsToEndPart2(new List<Cave>(visited), (doubleVisitUsed || (!neighbor.BigCave && visited.Contains(neighbor)))))
                    {
                        List<Cave> path = new() { this };
                        path.AddRange(neighborPath);
                        paths.Add(path);
                    }
                }
                return paths;
            }

            public override string ToString()
            {
                return Name ?? "";
            }
        }

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Cave.AddCaves(input.ReadLine()!);
                }
            }

            return Cave.Caves.Single(c => c.Name == "start").PathsToEnd(new List<Cave>()).Count.ToString();
        }

        public override string Part2()
        {
            return Cave.Caves.Single(c => c.Name == "start").PathsToEndPart2(new List<Cave>(), false).Count.ToString();
        }
    }
}