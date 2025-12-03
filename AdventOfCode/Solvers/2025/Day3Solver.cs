using System.Text;

namespace AdventOfCode.Solvers._2025;

internal class Day3Solver : AbstractSolver
{
    public override string Part1()
    {
        var result = 0;

        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;

                var firstNum = GetHighest(line[..^1]);
                var secondNum = GetHighest(line[(firstNum.idx + 1)..]);
                result += int.Parse(firstNum.num + secondNum.num);
            }
        }

        return result.ToString();
    }

    private static (string num, int idx) GetHighest(string input, int idxOffset = 0)
    {
        (string num, int idx) highest = new()
        {
            num = "0"
        };
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] > highest.num[0])
            {
                highest.num = input[i].ToString();
                highest.idx = i + idxOffset;
            }
        }

        return highest;
    }

    public override string Part2()
    {
        long result = 0;
        int batteriesToJolt = 12;

        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                List<(string num, int idx)> digits = new();

                for (var i = 0; i < batteriesToJolt; i++)
                {
                    digits.Add(i == 0
                        ? GetHighest(line[..^(batteriesToJolt - 1)])
                        : GetHighest(line[(digits.Last().idx + 1)..^((batteriesToJolt - 1) - i)],
                            (digits.Last().idx + 1)));
                }

                StringBuilder joltage = new();
                foreach (var digit in digits) joltage.Append(digit.num);

                result += long.Parse(joltage.ToString());
            }
        }
        return result.ToString();
    }
}