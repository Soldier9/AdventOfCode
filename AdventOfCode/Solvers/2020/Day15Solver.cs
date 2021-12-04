using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day15Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int> numbers = new List<int>();
            using (var input = File.OpenText(InputFile))
            {
                foreach (string n in input.ReadLine().Split(','))
                {
                    numbers.Add(int.Parse(n));
                }
            }

            while (numbers.Count < 2020)
            {
                bool alreadySaid = false;
                for (int previousRound = numbers.Count - 2; previousRound > -1; previousRound--)
                {
                    if (numbers[previousRound] == numbers[numbers.Count - 1])
                    {
                        numbers.Add(numbers.Count - 1 - previousRound);
                        alreadySaid = true;
                        break;
                    }
                }
                if (!alreadySaid) numbers.Add(0);
            }

            return numbers[numbers.Count - 1].ToString();
        }

        public override string Part2()
        {
            Dictionary<int, (int, int)> numbers = new Dictionary<int, (int, int)>();
            int round = 1;
            int lastNum = 0;

            using (var input = File.OpenText(InputFile))
            {
                foreach (string n in input.ReadLine().Split(','))
                {
                    lastNum = int.Parse(n);
                    numbers.Add(lastNum, (0, round));
                    round++;
                }
            }

            while (round < 30000001)
            {
                int nextNum = 0;
                if (numbers.ContainsKey(lastNum) && numbers[lastNum].Item1 != 0)
                {
                    nextNum = numbers[lastNum].Item2 - numbers[lastNum].Item1;
                }

                if (numbers.ContainsKey(nextNum))
                {
                    numbers[nextNum] = (numbers[nextNum].Item2, round);
                }
                else
                {
                    numbers.Add(nextNum, (0, round));
                }
                lastNum = nextNum;
                round++;
            }

            return lastNum.ToString();
        }
    }
}