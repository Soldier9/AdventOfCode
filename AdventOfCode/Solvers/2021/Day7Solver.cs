namespace AdventOfCode.Solvers.Year2021
{
    class Day7Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int> startPositions = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                startPositions = input.ReadLine()!.Split(',').Select(p => int.Parse(p)).ToList();
            }

            int bestResult = int.MaxValue;
            for (int possiblePosition = startPositions.Min(); possiblePosition <= startPositions.Max(); possiblePosition++)
            {
                int currentResult = 0;
                foreach (int crab in startPositions)
                {
                    currentResult += Math.Abs(crab - possiblePosition);
                    if (currentResult > bestResult) break;
                }
                if (currentResult < bestResult) bestResult = currentResult;
            }

            return bestResult.ToString();
        }

        public override string Part2()
        {
            List<int> startPositions = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                startPositions = input.ReadLine()!.Split(',').Select(p => int.Parse(p)).ToList();
            }

            long bestResult = long.MaxValue;
            for (int possiblePosition = startPositions.Min(); possiblePosition <= startPositions.Max(); possiblePosition++)
            {
                long currentResult = 0;
                foreach (int crab in startPositions)
                {
                    for (int m = Math.Abs(crab - possiblePosition); m > 0; m--)
                    {
                        currentResult += m;
                        if (currentResult > bestResult) break;
                    }
                    if (currentResult > bestResult) break;
                }
                if (currentResult < bestResult) bestResult = currentResult;
            }

            return bestResult.ToString();
        }
    }
}
