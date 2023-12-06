using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solvers.Year2023
{
    class Day6Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<(int time, int record)> races = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex numberParser = new Regex(@"\d+");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    foreach (Match number in numberParser.Matches(line))
                    {
                        races.Add((int.Parse(number.Value), -1));
                    }

                    line = input.ReadLine()!;
                    MatchCollection numbers = numberParser.Matches(line);
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        races[i] = (races[i].time, int.Parse(numbers[i].Value));
                    }
                }
            }

            int result = 1;
            foreach ((int time, int record) race in races)
            {
                int waysToWin = 0;
                for(int n = 0; n <= race.time; n++)
                {
                    if (n * (race.time - n) > race.record) waysToWin++;
                }
                result *= waysToWin;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            List<(long time, long record)> races = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex numberParser = new Regex(@"\d+");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    foreach (Match number in numberParser.Matches(line.Replace(" ", ""))) {
                        races.Add((long.Parse(number.Value), -1));
                    }

                    line = input.ReadLine()!;
                    MatchCollection numbers = numberParser.Matches(line.Replace(" ", ""));
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        races[i] = (races[i].time, long.Parse(numbers[i].Value));
                    }
                }
            }

            long result = 1;
            foreach ((long time, long record) race in races)
            {
                long waysToWin = 0;
                for (long n = 0; n <= race.time; n++)
                {
                    if (n * (race.time - n) > race.record) waysToWin++;
                }
                result *= waysToWin;
            }

            return result.ToString();
        }
    }
}
