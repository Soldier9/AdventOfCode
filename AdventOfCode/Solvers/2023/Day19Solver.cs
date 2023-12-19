using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day19Solver : AbstractSolver
    {
        class Condition
        {
            public char rating;
            public char comparator;
            public int value;
            public string resultIfFulfilled;

            private static readonly Regex parser = new(@"([xmas])([<>])(-?\d+):(\w+)");
            public Condition(string condString)
            {
                Match parsed = parser.Match(condString);
                if (parsed.Success)
                {
                    rating = parsed.Groups[1].Value[0];
                    comparator = parsed.Groups[2].Value[0];
                    value = int.Parse(parsed.Groups[3].Value);
                    resultIfFulfilled = parsed.Groups[4].Value;
                }
                else
                {
                    resultIfFulfilled = condString;
                }
            }

            public bool IsFullfilled((int x, int m, int a, int s) part)
            {
                int comparedValue = rating switch
                {
                    'x' => part.x,
                    'm' => part.m,
                    'a' => part.a,
                    's' => part.s,
                    _ => 0
                };
                return comparator switch
                {
                    '<' => comparedValue < value,
                    '>' => comparedValue > value,
                    _ => true,
                };
            }
        }

        readonly Dictionary<string, List<Condition>> workflows = new();
        readonly List<(int x, int m, int a, int s)> parts = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parseParts = false;
                while (!input.EndOfStream)
                {
                    if (!parseParts)
                    {
                        string[] line = input.ReadLine()!.TrimEnd('}').Split("{,".ToCharArray());
                        if (line.Length == 1)
                        {
                            parseParts = true;
                            continue;
                        }
                        workflows.Add(line[0], new());
                        for (int i = 1; i < line.Length; i++)
                        {
                            workflows[line[0]].Add(new Condition(line[i]));
                        }
                    }
                    else
                    {
                        Regex parseNums = new(@"-?\d+");
                        MatchCollection parsed = parseNums.Matches(input.ReadLine()!);
                        parts.Add((int.Parse(parsed[0].Value), int.Parse(parsed[1].Value), int.Parse(parsed[2].Value), int.Parse(parsed[3].Value)));
                    }
                }
            }

            long result = 0;
            foreach ((int x, int m, int a, int s) part in parts)
            {
                List<Condition> workflow = workflows["in"];
                while (workflow.Count > 0)
                {
                    foreach (Condition condition in workflow)
                    {
                        if (condition.IsFullfilled(part))
                        {
                            switch (condition.resultIfFulfilled)
                            {
                                case "A":
                                    result += part.x + part.m + part.a + part.s;
                                    workflow = new();
                                    break;
                                case "R":
                                    workflow = new();
                                    break;
                                default:
                                    workflow = workflows[condition.resultIfFulfilled];
                                    break;
                            }
                            break;
                        }
                    }
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            (long x, long m, long a, long s) minValues = (1, 1, 1, 1);
            (long x, long m, long a, long s) maxValues = (4000, 4000, 4000, 4000);

            return GetNumberOfPossibleParts("in", minValues, maxValues).ToString();
        }

        long GetNumberOfPossibleParts(string startWorkflow, (long x, long m, long a, long s) minValues, (long x, long m, long a, long s) maxValues)
        {
            long result = 0;
            foreach (Condition condition in workflows[startWorkflow])
            {
                (long x, long m, long a, long s) fulfilledMinValues = minValues;
                (long x, long m, long a, long s) fulfilledMaxValues = maxValues;
                switch (condition.comparator)
                {
                    case '>':
                        switch (condition.rating)
                        {
                            case 'x': fulfilledMinValues.x = Math.Max(minValues.x, condition.value + 1); break;
                            case 'm': fulfilledMinValues.m = Math.Max(minValues.m, condition.value + 1); break;
                            case 'a': fulfilledMinValues.a = Math.Max(minValues.a, condition.value + 1); break;
                            case 's': fulfilledMinValues.s = Math.Max(minValues.s, condition.value + 1); break;
                        }
                        break;
                    case '<':
                        switch (condition.rating)
                        {
                            case 'x': fulfilledMaxValues.x = Math.Min(maxValues.x, condition.value - 1); break;
                            case 'm': fulfilledMaxValues.m = Math.Min(maxValues.m, condition.value - 1); break;
                            case 'a': fulfilledMaxValues.a = Math.Min(maxValues.a, condition.value - 1); break;
                            case 's': fulfilledMaxValues.s = Math.Min(maxValues.s, condition.value - 1); break;
                        }
                        break;
                }
                switch (condition.resultIfFulfilled)
                {
                    case "R": break;
                    case "A":
                        result += ((fulfilledMaxValues.x - fulfilledMinValues.x + 1)
                            * (fulfilledMaxValues.m - fulfilledMinValues.m + 1)
                            * (fulfilledMaxValues.a - fulfilledMinValues.a + 1)
                            * (fulfilledMaxValues.s - fulfilledMinValues.s + 1));
                        break;
                    default:
                        result += GetNumberOfPossibleParts(condition.resultIfFulfilled, fulfilledMinValues, fulfilledMaxValues);
                        break;
                }
                switch (condition.comparator)
                {
                    case '<':
                        switch (condition.rating)
                        {
                            case 'x': minValues.x = Math.Max(minValues.x, condition.value); break;
                            case 'm': minValues.m = Math.Max(minValues.m, condition.value); break;
                            case 'a': minValues.a = Math.Max(minValues.a, condition.value); break;
                            case 's': minValues.s = Math.Max(minValues.s, condition.value); break;
                        }
                        break;
                    case '>':
                        switch (condition.rating)
                        {
                            case 'x': maxValues.x = Math.Min(maxValues.x, condition.value); break;
                            case 'm': maxValues.m = Math.Min(maxValues.m, condition.value); break;
                            case 'a': maxValues.a = Math.Min(maxValues.a, condition.value); break;
                            case 's': maxValues.s = Math.Min(maxValues.s, condition.value); break;
                        }
                        break;
                }
            }
            return result;
        }
    }
}
