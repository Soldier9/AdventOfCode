namespace AdventOfCode.Solvers.Year2020
{
    class Day9Solver : AbstractSolver
    {
        readonly List<long> buffer = new();
        long firstInvalidNumber;

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    long nextNum = long.Parse(input.ReadLine()!);
                    if (buffer.Count >= 25)
                    {
                        bool isValid = false;
                        for (int i = buffer.Count - 25; i < buffer.Count; i++)
                        {
                            for (int j = buffer.Count - 25; j < buffer.Count; j++)
                            {
                                if (j == i) continue;
                                if (buffer[j] + buffer[i] == nextNum)
                                {
                                    isValid = true;
                                    break;
                                }
                            }
                            if (isValid) break;
                        }
                        if (!isValid)
                        {
                            firstInvalidNumber = nextNum;
                            return firstInvalidNumber.ToString();
                        }
                    }
                    buffer.Add(nextNum);
                }
            }

            return "No solution found";
        }

        public override string Part2()
        {
            for (int i = 0; i < buffer.Count; i++)
            {
                long sum = buffer[i];
                for (int j = i + 1; j < buffer.Count; j++)
                {
                    sum += buffer[j];
                    if (sum > firstInvalidNumber) break;

                    if (sum == firstInvalidNumber)
                    {
                        long smallest = buffer[i];
                        long largest = buffer[i];
                        for (int x = i; x <= j; x++)
                        {
                            smallest = Math.Min(smallest, buffer[x]);
                            largest = Math.Max(largest, buffer[x]);
                        }
                        return (smallest + largest).ToString();
                    }
                }
            }

            return "No solution found";
        }
    }
}