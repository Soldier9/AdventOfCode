using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2024
{
    class Day3Solver : AbstractSolver
    {
        public override string Part1()
        {
            long result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    MatchCollection instructions = Regex.Matches(input.ReadLine()!, @"mul\((\d{1,3}),(\d{1,3})\)");
                    foreach (Match match in instructions)
                    {
                        result += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                    }
                }
            }
            return result.ToString();
        }

        public override string Part2()
        {
            long result = 0;
            bool doMul = true;

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    MatchCollection instructions = Regex.Matches(input.ReadLine()!, @"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)");
                    foreach (Match match in instructions)
                    {
                        switch(match.Groups[0].Value)
                        {
                            case "do()":    doMul = true; continue;
                            case "don't()": doMul = false; continue;
                        }
                        if(doMul) result += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                    }
                }
            }
            return result.ToString();
        }
    }
}
