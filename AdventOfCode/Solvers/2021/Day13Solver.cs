using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day13Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<(int x, int y)> dots = new List<(int x, int y)>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.Contains(","))
                    {
                        string[] split = line.Split(',');
                        dots.Add((int.Parse(split[0]), int.Parse(split[1])));
                    }
                    else if (line.StartsWith("fold along "))
                    {
                        string[] split = line.Substring(11).Split('=');
                        string axis = split[0];
                        int pos = int.Parse(split[1]);

                        switch (split[0])
                        {
                            case "x":
                                for (int i = 0; i < dots.Count; i++) if (dots[i].x > pos) dots[i] = (pos - (dots[i].x - pos), dots[i].y);
                                break;

                            case "y":
                                for (int i = 0; i < dots.Count; i++) if (dots[i].y > pos) dots[i] = (dots[i].x, pos - (dots[i].y - pos));
                                break;
                        }

                        HashSet<(int x, int y)> unique = new HashSet<(int x, int y)>();
                        foreach ((int, int) dot in dots) if (!unique.Contains(dot)) unique.Add(dot);
                        dots = unique.ToList();
                        return dots.Count().ToString();
                    }

                }
            }

            return ":(";
        }

        public override string Part2()
        {
            List<(int x, int y)> dots = new List<(int x, int y)>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.Contains(","))
                    {
                        string[] split = line.Split(',');
                        dots.Add((int.Parse(split[0]), int.Parse(split[1])));
                    }
                    else if (line.StartsWith("fold along "))
                    {
                        string[] split = line.Substring(11).Split('=');
                        string axis = split[0];
                        int pos = int.Parse(split[1]);

                        switch (split[0])
                        {
                            case "x":
                                for (int i = 0; i < dots.Count; i++) if (dots[i].x > pos) dots[i] = (pos - (dots[i].x - pos), dots[i].y);
                                break;

                            case "y":
                                for (int i = 0; i < dots.Count; i++) if (dots[i].y > pos) dots[i] = (dots[i].x, pos - (dots[i].y - pos));
                                break;
                        }

                        HashSet<(int x, int y)> unique = new HashSet<(int x, int y)>();
                        foreach ((int, int) dot in dots) if (!unique.Contains(dot)) unique.Add(dot);
                        dots = unique.ToList();
                    }
                }
            }

            foreach ((int x, int y) dot in dots)
            {
                Console.SetCursorPosition(dot.x, dot.y + 7);
                Console.Write("#");
            }
            Console.SetCursorPosition(0, 6);
            return "";
        }
    }
}
