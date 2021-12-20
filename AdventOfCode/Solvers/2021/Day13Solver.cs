using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day13Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<(int x, int y)> dots = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Contains(','))
                    {
                        string[] split = line.Split(',');
                        dots.Add((int.Parse(split[0]), int.Parse(split[1])));
                    }
                    else if (line.StartsWith("fold along "))
                    {
                        string[] split = line[11..].Split('=');
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

                        HashSet<(int x, int y)> unique = new();
                        foreach ((int, int) dot in dots) if (!unique.Contains(dot)) _ = unique.Add(dot);
                        dots = unique.ToList();
                        return dots.Count.ToString();
                    }
                }
            }

            return ":(";
        }

        public override string Part2()
        {
            List<(int x, int y)> dots = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Contains(','))
                    {
                        string[] split = line.Split(',');
                        dots.Add((int.Parse(split[0]), int.Parse(split[1])));
                    }
                    else if (line.StartsWith("fold along "))
                    {
                        string[] split = line[11..].Split('=');
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

                        HashSet<(int x, int y)> unique = new();
                        foreach ((int, int) dot in dots) if (!unique.Contains(dot)) _ = unique.Add(dot);
                        dots = unique.ToList();
                    }
                }
            }

            (int min, int max) xBounds = (dots.MinBy(d => d.x).x, dots.MaxBy(d => d.x).x);
            (int min, int max) yBounds = (dots.MinBy(d => d.y).y, dots.MaxBy(d => d.y).y);
            StringBuilder sb = new();
            for (int y = yBounds.min; y <= yBounds.max; y++)
            {
                for (int x = xBounds.min; x <= xBounds.max; x++)
                {
                    _ = sb.Append(dots.Contains((x, y)) ? '#' : ' ');
                }
                _ = sb.Append("\r\n");
            }
            Program.PrintData(sb.ToString(), 0, true);
            //foreach ((int x, int y) in dots)
            //{
            //    Console.SetCursorPosition(x, y + 7);
            //    Console.Write("#");
            //}
            //Console.SetCursorPosition(0, 6);
            return "Look below";
        }
    }
}
