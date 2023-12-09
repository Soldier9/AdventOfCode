using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day9Solver : AbstractSolver
    {
        List<List<int>> sequences = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex numberParser = new Regex(@"-?\d+");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    List<int> sequence = new();
                    foreach(Match num in numberParser.Matches(line))
                    {
                        sequence.Add(int.Parse(num.Value));
                    }
                    sequences.Add(sequence);
                }
            }

            int result = 0;
            foreach(List<int> sequence in sequences)
            {
                List<List<int>> diffs = new();
                diffs.Add(sequence);
                List<int> lastDiff = diffs.Last();
                while (lastDiff.Any(n => n != 0))
                {
                    List<int> newDiff = new();
                    for(int i = 1; i < lastDiff.Count; i++)
                    {
                        newDiff.Add(lastDiff[i] - lastDiff[i - 1]);
                    }
                    diffs.Add(newDiff);
                    lastDiff = newDiff;
                }


                int nextDiff = diffs.Last().Last();
                for(int i = diffs.Count - 2; i >= 0; i--)
                {
                    nextDiff = diffs[i].Last() + nextDiff;
                }
                result += nextDiff;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;
            foreach (List<int> sequence in sequences)
            {
                List<List<int>> diffs = new();
                diffs.Add(sequence);
                List<int> lastDiff = diffs.Last();
                while (lastDiff.Any(n => n != 0))
                {
                    List<int> newDiff = new();
                    for (int i = 1; i < lastDiff.Count; i++)
                    {
                        newDiff.Add(lastDiff[i] - lastDiff[i - 1]);
                    }
                    diffs.Add(newDiff);
                    lastDiff = newDiff;
                }

                int nextDiff = diffs.Last().First();
                for (int i = diffs.Count - 2; i >= 0; i--)
                {
                    nextDiff = diffs[i].First() - nextDiff;
                }
                result += nextDiff;
            }

            return result.ToString();
        }
    }
}