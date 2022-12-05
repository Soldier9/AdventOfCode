using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day5Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<Stack<char>> stacks = new();
            for (int i = 1; i <= 9; i++) stacks.Add(new());

            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingStacks = true;
                Regex findCrates = new Regex(@"\[\w\]");
                Regex parseInstr = new Regex(@"\d+");

                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (parsingStacks)
                    {
                        foreach (Match match in findCrates.Matches(line)) stacks[(match.Index / 4)].Push(match.Value[1]);
                        if (line.Length == 0)
                        {
                            parsingStacks = false;
                            for (int i = 0; i < stacks.Count; i++) stacks[i] = new(stacks[i]); // This reverses the stacks
                        }
                    }
                    else
                    {
                        MatchCollection move = parseInstr.Matches(line);
                        for (int i = 0; i < int.Parse(move[0].Value); i++) stacks[int.Parse(move[2].Value) - 1].Push(stacks[int.Parse(move[1].Value) - 1].Pop());
                    }
                }
            }

            string result = "";
            foreach (Stack<char> stack in stacks) result += stack.Pop();
            return result.ToString();
        }

        public override string Part2()
        {
            List<Stack<char>> stacks = new();
            for (int i = 1; i <= 9; i++) stacks.Add(new());

            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingStacks = true;
                Regex findCrates = new Regex(@"\[\w\]");
                Regex parseInstr = new Regex(@"\d+");

                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (parsingStacks)
                    {
                        foreach (Match match in findCrates.Matches(line)) stacks[(match.Index / 4)].Push(match.Value[1]);
                        if (line.Length == 0)
                        {
                            parsingStacks = false;
                            for (int i = 0; i < stacks.Count; i++) stacks[i] = new(stacks[i]); // This reverses the stacks
                        }
                    }
                    else
                    {
                        MatchCollection move = parseInstr.Matches(line);
                        List<char> beingMoved = new();
                        for (int i = 0; i < int.Parse(move[0].Value); i++) beingMoved.Add(stacks[int.Parse(move[1].Value) - 1].Pop());
                        for (int i = beingMoved.Count - 1; i > -1; i--) stacks[int.Parse(move[2].Value) - 1].Push(beingMoved[i]);
                    }
                }
            }

            string result = "";
            foreach (Stack<char> stack in stacks) result += stack.Pop();
            return result.ToString();
        }
    }
}
