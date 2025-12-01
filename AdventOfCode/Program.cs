using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers;

namespace AdventOfCode;

internal static class Program
{
    private static IEnumerable<AbstractSolver>? _solvers;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    public static bool VisualizationEnabled;
    public static bool ExtendedVisualization;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    private static int _lastVisualizationLine = -1;

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private static void Main()
    {
        var year = 0;
        var day = 0;
        // ReSharper disable once RedundantAssignment
        bool doList = false;

        do
        {
#if !DEBUG
            doList = false;
            Console.WriteLine("Input YYYY-DD to run or anything else to run latest solver.");
            Console.WriteLine("Input \"list\" to list available solvers.");
            Console.Write(
                "End with ! to enable visualization or !! for extended visualization (if either is available): ");
            string[] inputs = Console.ReadLine()!.Split('-');
            Console.Clear();
            if (inputs[0] == "list")
            {
                doList = true;
            }
            else
            {
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
                    _ = int.TryParse(inputs[0], out year);
                    _ = int.TryParse(inputs[1], out day);
                }
            }
#endif

            _solvers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType is not null && t.BaseType!.Name == "AbstractSolver")
                .Select(s => (AbstractSolver)Activator.CreateInstance(s)!)
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Day)
                .ThenBy(s => s.PrioritizedSolver);

            if (doList)
                foreach (var s in _solvers)
                    Console.WriteLine(s.Year + "-" + s.Day + " " + (s.HasVisualization ? "!" : "") +
                                      (s.HasExtendedVisualization ? "!" : ""));
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        } while (doList);

        var solver = _solvers.SingleOrDefault(s => s.Year == year && s.Day == day, _solvers.Last());
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
                var line = Console.CursorTop;
                Console.Write("Press any key to run part 2...");
                _ = Console.ReadKey();
                for (var i = line; i <= _lastVisualizationLine; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(new string(' ', Console.BufferWidth));
                }

                Console.SetCursorPosition(0, line);
            }
            else
            {
                var timer = Stopwatch.StartNew();
                var result = solver.Part1();
                timer.Stop();
                Console.WriteLine("Solution to Part 1: {0} ({1:##,0.#} ms)", result,
                    // ReSharper disable once PossibleLossOfFraction
                    (double)timer.ElapsedTicks / (Stopwatch.Frequency / 1000));
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
                var timer = Stopwatch.StartNew();
                var result = solver.Part2();
                timer.Stop();
                Console.WriteLine("Solution to Part 2: {0} ({1:##,0.#} ms)", result,
                    // ReSharper disable once PossibleLossOfFraction
                    (double)timer.ElapsedTicks / (Stopwatch.Frequency / 1000));
            }
        }
        catch (NotImplementedException)
        {
            Console.WriteLine("Solution not implemented");
        }

        _ = Console.ReadLine();
        var finalCursorLine = Console.CursorTop;
        for (var i = finalCursorLine; i <= _lastVisualizationLine; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.BufferWidth));
        }

        Console.SetCursorPosition(0, finalCursorLine);
    }

    public static Dictionary<(int x, int y), char> CreateDictFromEnumerable(IEnumerable<(int x, int y)> charLocs,
        char charToUse = '#', Dictionary<(int x, int y), char>? baseDict = null)
    {
        var dict = baseDict ?? [];
        foreach (var x in charLocs) dict[x] = charToUse;
        return dict;
    }

    public static string CreateStringFromSet(HashSet<(int x, int y)> charLocs, char charToUse = '#')
    {
        return CreateStringFromDict(CreateDictFromEnumerable(charLocs, charToUse));
    }

    public static string CreateStringFromDict(Dictionary<(int x, int y), char> dict)
    {
        return CreateStringFromDict(dict, new Dictionary<char, string>());
    }

    public static string CreateStringFromDict(Dictionary<(int x, int y), char> dict,
        Dictionary<char, string> decorations, int? minX = null, int? minY = null, int? maxX = null, int? maxY = null,
        char background = ' ')
    {
        StringBuilder sb = new();

        var min = (dict.MinBy(e => e.Key.x).Key.x, dict.MinBy(e => e.Key.y).Key.y);
        if (minX != null && minX < min.x) min.x = (int)minX;
        if (minY != null && minY < min.y) min.y = (int)minY;

        var max = (dict.MaxBy(e => e.Key.x).Key.x, dict.MaxBy(e => e.Key.y).Key.y);
        if (maxX != null && maxX > max.x) max.x = (int)maxX;
        if (maxY != null && maxY > max.y) max.y = (int)maxY;

        for (var y = min.y; y <= max.y; y++)
        {
            for (var x = min.x; x <= max.x; x++)
            {
                var newChar = dict.ContainsKey((x, y)) ? dict[(x, y)] : background;

                // ReSharper disable once CanSimplifyDictionaryLookupWithTryGetValue
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (decorations.ContainsKey(newChar))
                    _ = sb.Append(decorations[newChar] + newChar + "\u001b[0m");
                else _ = sb.Append(newChar);
            }

            _ = sb.Append("\r\n");
        }

        return sb.ToString();
    }

    public static void PrintData(string output, int delayAfter = 0, bool printWithVisualizationDisabled = false,
        bool cropToConsoleSize = false)
    {
        if (!VisualizationEnabled && !printWithVisualizationDisabled) return;
        (int x, int y) cursor = (Console.CursorLeft, Console.CursorTop);
        Console.CursorVisible = false;
        Console.SetCursorPosition(0, cursor.y + 2);
        if (cropToConsoleSize)
        {
            var lines = output.Split("\r\n");
            var linesToPrint = Math.Min(Console.WindowHeight - cursor.y - 3, lines.Length);
            StringBuilder sb = new();

            Regex findAnsiChunks = new(@"((\u001b\[[^\e]+m)*[^\e]){0," + Console.WindowWidth + @"}(\u001b\[[^\e]+m)*");
            for (var l = 0; l < linesToPrint; l++)
            {
                var chunksWithAnsi = findAnsiChunks.Matches(lines[l]);
                _ = sb.Append(chunksWithAnsi[0].Value);
                for (var i = 1; i < chunksWithAnsi.Count; i++)
                    if (chunksWithAnsi[i].Groups[3].Value == "\u001b[0m")
                    {
                        _ = sb.Append("\u001b[0m");
                        break;
                    }

                _ = sb.Append("\r\n");
            }

            Console.Write(sb.ToString());
        }
        else
        {
            Console.Write(output);
        }

        _lastVisualizationLine = Console.CursorTop;
        Console.SetCursorPosition(cursor.x, cursor.y);
        Console.CursorVisible = true;

        if (delayAfter > 0) Thread.Sleep(delayAfter);
    }
}