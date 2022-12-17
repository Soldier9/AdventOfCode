using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {
        static IEnumerable<AbstractSolver>? Solvers;
        public static bool VisualizationEnabled = false;
        public static bool ExtendedVisualization = false;

        private static int LastVisualizationLine = -1;
        static void Main()
        {
            int Year = 0;
            int Day = 0;
            #if !DEBUG
            Console.WriteLine("Input YYYY-DD to run or anything else to run latest solver.");
            Console.Write("End with ! to enable visualization or !! for extended visualization (if either is available): ");
            string[] inputs = Console.ReadLine()!.Split('-');
            Console.Clear();
            if (inputs[^1].Length > 0 && inputs[^1][^1] == '!')
            {
                VisualizationEnabled = true;
                inputs[^1] = inputs[^1][..^1];
                if (inputs[^1].Length > 0 && inputs[^1][^1] == '!')
                {
                    ExtendedVisualization = true;
                    inputs[^1] = inputs[^1][..^1];
                }
            }
            if (inputs.Length == 2)
            {
                _ = int.TryParse(inputs[0], out Year);
                _ = int.TryParse(inputs[1], out Day);
            }
            #endif

            Solvers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType is not null && t.BaseType!.Name == "AbstractSolver")
                .Select(s => (AbstractSolver)Activator.CreateInstance(s)!)
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Day)
                .ThenBy(s => s.PrioritizedSolver)!;

            AbstractSolver solver = Solvers.SingleOrDefault(s => s.Year == Year && s.Day == Day, Solvers.Last());
            Console.WriteLine("Advent of Code {0} Day {1}"
                + (solver.HasExtendedVisualization ? " - Extended Visualization available for this solver"
                : solver.HasVisualization ? " - Visualization available for this solver" : ""),
                solver.Year, solver.Day);

            Console.WriteLine("\r\nSolving Part 1");
            try
            {
                if (VisualizationEnabled)
                {
                    Console.WriteLine("Solution to Part 1: {0}", solver.Part1());
                    int line = Console.CursorTop;
                    Console.Write("Press any key to run part 2...");
                    _ = Console.ReadKey();
                    for (int i = line; i <= LastVisualizationLine; i++)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(new string(' ', Console.BufferWidth));
                    }
                    Console.SetCursorPosition(0, line);
                }
                else
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    string result = solver.Part1();
                    timer.Stop();
                    Console.WriteLine("Solution to Part 1: {0} ({1:##,0.#} ms)", result, (double)timer.ElapsedTicks / (Stopwatch.Frequency / 1000));
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Solution not implemented");
            }

            Console.WriteLine("\r\nSolving Part 2");
            try
            {
                if (VisualizationEnabled)
                {
                    Console.WriteLine("Solution to Part 2: {0}.", solver.Part2());
                }
                else
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    string result = solver.Part2();
                    timer.Stop();
                    Console.WriteLine("Solution to Part 2: {0} ({1:##,0.#} ms)", result, (double)timer.ElapsedTicks / (Stopwatch.Frequency / 1000));
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Solution not implemented");
            }
            _ = Console.ReadLine();
            int finalCursorLine = Console.CursorTop;
            for (int i = finalCursorLine; i <= LastVisualizationLine; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.BufferWidth));
            }
            Console.SetCursorPosition(0, finalCursorLine);
        }

        public static string CreateStringFromDict(HashSet<(int x, int y)> charLocs, char charToUse = '#')
        {
            Dictionary<(int x, int y), char> dict = new();
            foreach ((int x, int y) x in charLocs) dict.Add(x, charToUse);
            return CreateStringFromDict(dict);
        }
        public static string CreateStringFromDict(Dictionary<(int x, int y), char> dict) => CreateStringFromDict(dict, new(), null);
        public static string CreateStringFromDict(Dictionary<(int x, int y), char> dict, Dictionary<char, string> decorations, int? minX = null, int? minY = null, int? maxX = null, int? maxY = null, char background = ' ')
        {
            StringBuilder sb = new();

            (int x, int y) min = (dict.MinBy(e => e.Key.x).Key.x, dict.MinBy(e => e.Key.y).Key.y);
            if (minX != null && minX < min.x) min.x = (int)minX;
            if (minY != null && minY < min.y) min.y = (int)minY;

            (int x, int y) max = (dict.MaxBy(e => e.Key.x).Key.x, dict.MaxBy(e => e.Key.y).Key.y);
            if (maxX != null && maxX > max.x) max.x = (int)maxX;
            if (maxY != null && maxY > max.y) max.y = (int)maxY;

            for (int y = min.y; y <= max.y; y++)
            {
                for(int x = min.x; x <= max.x; x++)
                {
                    char newChar = dict.ContainsKey((x, y))? dict[(x, y)] : background;

                    if(decorations.ContainsKey(newChar))
                    {
                        _ = sb.Append(decorations[newChar] + newChar + "\u001b[0m");
                    } else _ = sb.Append(newChar);
                }
                _ = sb.Append("\r\n");
            }

            return sb.ToString();
        }

        public static void PrintData(string output, int delayAfter = 0, bool printWithVisualizationDisabled = false, bool cropToConsoleSize = false)
        {
            if (!VisualizationEnabled && !printWithVisualizationDisabled) return;
            (int x, int y) cursor = (Console.CursorLeft, Console.CursorTop);
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, cursor.y + 2);
            if (cropToConsoleSize)
            {
                string[] lines = output.Split("\r\n");
                int linesToPrint = Math.Min(Console.WindowHeight - cursor.y - 3, lines.Length);
                StringBuilder sb = new();

                Regex findAnsiChunks = new(@"((\u001b\[[^\e]+m)*[^\e]){0," + Console.WindowWidth + @"}(\u001b\[[^\e]+m)*");
                for (int l = 0; l < linesToPrint; l++)
                {
                    MatchCollection chunksWithAnsi = findAnsiChunks.Matches(lines[l]);
                    _ = sb.Append(chunksWithAnsi[0].Value);
                    for (int i = 1; i < chunksWithAnsi.Count; i++)
                    {
                        if (chunksWithAnsi[i].Groups[3].Value == "\u001b[0m")
                        {
                            _ = sb.Append("\u001b[0m");
                            break;
                        }
                    }
                    _ = sb.Append("\r\n");
                }
                Console.Write(sb.ToString());
            }
            else
            {
                Console.Write(output);
            }
            LastVisualizationLine = Console.CursorTop;
            Console.SetCursorPosition(cursor.x, cursor.y);
            Console.CursorVisible = true;

            if (delayAfter > 0) Thread.Sleep(delayAfter);
        }
    }
}
