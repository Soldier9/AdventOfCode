using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2021
{
    class Day10Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    bool lineIsCorrupt = false;
                    Stack<char> chunk = new Stack<char>();
                    foreach (char c in input.ReadLine())
                    {
                        switch (c)
                        {
                            case '(': chunk.Push(')'); break;
                            case '[': chunk.Push(']'); break;
                            case '{': chunk.Push('}'); break;
                            case '<': chunk.Push('>'); break;
                            default:
                                if (chunk.Pop() != c)
                                {
                                    switch (c)
                                    {
                                        case ')': result += 3; break;
                                        case ']': result += 57; break;
                                        case '}': result += 1197; break;
                                        case '>': result += 25137; break;
                                    }
                                    lineIsCorrupt = true;
                                }
                                break;
                        }
                        if (lineIsCorrupt) break;
                    }
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            List<Int64> scores = new List<Int64>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Int64 score = 0;
                    bool lineIsCorrupt = false;
                    Stack<char> chunk = new Stack<char>();
                    foreach (char c in input.ReadLine())
                    {
                        switch (c)
                        {
                            case '(': chunk.Push(')'); break;
                            case '[': chunk.Push(']'); break;
                            case '{': chunk.Push('}'); break;
                            case '<': chunk.Push('>'); break;
                            default:
                                if (chunk.Pop() != c) lineIsCorrupt = true;
                                break;
                        }
                        if (lineIsCorrupt)
                        {
                            chunk.Clear();
                            break;
                        }
                    }
                    while (chunk.Count > 0)
                    {
                        score *= 5;
                        switch (chunk.Pop())
                        {
                            case ')': score += 1; break;
                            case ']': score += 2; break;
                            case '}': score += 3; break;
                            case '>': score += 4; break;
                        }
                    }
                    if (score > 0) scores.Add(score);
                }
            }

            scores.Sort();
            int middle = scores.Count / 2;
            return scores[middle].ToString();
        }
    }
}
