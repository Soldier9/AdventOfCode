using System.Linq;

namespace AdventOfCode.Solvers.Year2024
{
    class Day22Solver : AbstractSolver
    {
        
        public override string Part1()
        {
            List<long> secretNumbers = [];
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    secretNumbers.Add(long.Parse(input.ReadLine()!));
                }
            }

            long result = 0;
            for (int i = 0; i < secretNumbers.Count; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    secretNumbers[i] = NextSecretNumber(secretNumbers[i]);
                }
                result += secretNumbers[i];
            }
            return result.ToString();
        }

        long NextSecretNumber(long input)
        {
            long output = (input ^ (input * 64)) % 16777216;
            output = (output ^ (output / 32)) % 16777216;
            output = (output ^ (output * 2048)) % 16777216;
            return output;
        }

        public override string Part2()
        {
            List<long> secretNumbers = [];
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    secretNumbers.Add(long.Parse(input.ReadLine()!));
                }
            }

            List<List<(long secretNumber, int price, int change)>> monkeys = [];
            Dictionary<(int n1, int n2, int n3, int n4), long> sequences = [];

            foreach (long secretNumber in secretNumbers)
            {
                monkeys.Add([(secretNumber, (int)(secretNumber % 10), 0)]);
                long nextSecretNumber = NextSecretNumber(secretNumber);
                HashSet<(int n1, int n2, int n3, int n4)> seenSequences = [];
                for (int i = 0; i < 2000; i++)
                {
                    int price = (int)(nextSecretNumber % 10);
                    monkeys[^1].Add((nextSecretNumber, price, price - monkeys[^1][^1].price));
                    if (i > 2)
                    {
                        int[] sequence = monkeys[^1][(i - 2)..(i + 2)].Select(p => p.change).ToArray();
                        if (!seenSequences.Contains((sequence[0], sequence[1], sequence[2], sequence[3])))
                        {
                            seenSequences.Add((sequence[0], sequence[1], sequence[2], sequence[3]));
                            if (!sequences.TryAdd((sequence[0], sequence[1], sequence[2], sequence[3]), price)) sequences[(sequence[0], sequence[1], sequence[2], sequence[3])] += price;
                        }
                    }
                    nextSecretNumber = NextSecretNumber(nextSecretNumber);
                }
            }

            long result = sequences.MaxBy(s => s.Value).Value;
            return result.ToString();
        }
    }
}
