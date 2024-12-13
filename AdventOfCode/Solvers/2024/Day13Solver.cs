using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2024
{
    class Day13Solver : AbstractSolver
    {
        class ClawMachine
        {
            public (long x, long y) ButtonA;
            public (long x, long y) ButtonB;
            public (long x, long y) PrizeAt;

            public Dictionary<(long x, long y), long> resultCache = [];
            public long CostToSolve((long x, long y) fromPos)
            {
                if (resultCache.TryGetValue(fromPos, out long cost)) return cost;

                if (fromPos.x > PrizeAt.x || fromPos.y > PrizeAt.y) cost = -1;
                else if (fromPos != PrizeAt)
                {
                    long pathA = CostToSolve(AddPos(fromPos, ButtonA));
                    long pathB = CostToSolve(AddPos(fromPos, ButtonB));

                    if (pathA < 0 && pathB < 0) cost = -1;
                    else if (pathA < 0) cost = 1 + pathB;
                    else if (pathB < 0) cost = 3 + pathA;
                    else cost = Math.Min(pathA + 3, pathB + 1);
                }
                resultCache.Add(fromPos, cost);
                return cost;
            }

            public long CostToSolveWithCramersRule()
            {
                long denom = (ButtonA.x * ButtonB.y) - (ButtonA.y * ButtonB.x);

                long aPresses = (PrizeAt.x * ButtonB.y - PrizeAt.y * ButtonB.x) / denom;
                long bPresses = (ButtonA.x * PrizeAt.y - ButtonA.y * PrizeAt.x) / denom;

                if (aPresses >= 0 && bPresses >= 0 && aPresses * ButtonA.x + bPresses * ButtonB.x == PrizeAt.x && aPresses * ButtonA.y + bPresses * ButtonB.y == PrizeAt.y)
                {
                    return 3 * (long)aPresses + (long)bPresses;
                }
                return 0;
            }

            private static (long x, long y) AddPos((long x, long y) a, (long x, long y) b)
            {
                return (a.x + b.x, a.y + b.y);
            }
        }

        List<ClawMachine> ClawMachines = [];

        public override string Part1()
        {
            Regex parseLine = new(@"(\w+)\:\sX[\+\=](\d+),\sY[\+\=](\d+)");
            using (StreamReader input = File.OpenText(InputFile))
            {
                ClawMachine clawMachine = new();
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Length == 0)
                    {
                        ClawMachines.Add(clawMachine);
                        clawMachine = new();
                    }
                    else
                    {
                        Match match = parseLine.Match(line);
                        switch (match.Groups[1].Value)
                        {
                            case "A": clawMachine.ButtonA = (long.Parse(match.Groups[2].Value), long.Parse(match.Groups[3].Value)); break;
                            case "B": clawMachine.ButtonB = (long.Parse(match.Groups[2].Value), long.Parse(match.Groups[3].Value)); break;
                            case "Prize": clawMachine.PrizeAt = (long.Parse(match.Groups[2].Value), long.Parse(match.Groups[3].Value)); break;
                        }
                    }
                }
                ClawMachines.Add(clawMachine);
            }

            long result = 0;
            foreach (ClawMachine clawMachine in ClawMachines)
            {
                long costToSolve = clawMachine.CostToSolve((0, 0));
                if (costToSolve > 0) result += costToSolve;
            }
            return result.ToString();
        }

        public override string Part2()
        {
            long result = 0;
            foreach (ClawMachine clawMachine in ClawMachines)
            {
                clawMachine.PrizeAt = (clawMachine.PrizeAt.x + 10000000000000, clawMachine.PrizeAt.y + 10000000000000);
                result += clawMachine.CostToSolveWithCramersRule();
            }
            return result.ToString();
        }
    }
}
