﻿using System.Diagnostics;
using System.Reflection;
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
            //#if !DEBUG
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
            //#endif

            Solvers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType!.Name == "AbstractSolver")
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
                Regex findAnsiChunks = new(@"(\u001b\[[^\e]+m)+([^\e]*)|(^[^\e]+)");
                for (int l = 0; l < linesToPrint; l++)
                {
                    MatchCollection chunksWithAnsi = findAnsiChunks.Matches(lines[l]);
                    int actualLength = 0;
                    int chunkNum = 0;
                    while (chunkNum < chunksWithAnsi.Count && actualLength < Console.WindowWidth)
                    {
                        actualLength += Math.Max(chunksWithAnsi[chunkNum].Groups[2].Length, chunksWithAnsi[chunkNum].Groups[3].Length);
                        chunkNum++;
                    }

                    for (int i = 0; i < chunkNum - 1; i++)
                    {
                        _ = sb.Append(chunksWithAnsi[i].Groups[0].Value);
                    }
                    int charsToCut = Math.Max(actualLength - Console.WindowWidth, 0);
                    _ = sb.Append(chunksWithAnsi[chunkNum - 1].Groups[0].Value[..^charsToCut]);

                    for (int i = chunkNum; i < chunksWithAnsi.Count; i++)
                    {
                        if (chunksWithAnsi[i].Groups[1].Value == "\u001b[0m")
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
