using System.Text;

namespace AdventOfCode.Solvers._2025;

internal class Day6Solver : AbstractSolver
{
    public override string Part1()
    {
        long result = 0;

        List<List<int>> problems = new();

        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var numbers = input.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < numbers.Length; i++)
                {
                    if (problems.Count == i) problems.Add(new());
                    try
                    {
                        problems[i].Add(int.Parse(numbers[i]));
                    }
                    catch (FormatException)
                    {
                        long problemResult = 0;
                        switch (numbers[i])
                        {
                            case "*":
                                problemResult = 1;
                                foreach (var number in problems[i]) problemResult *= number;
                                break;
                            case "+":
                                foreach (var number in problems[i]) problemResult += number;
                                break;
                        }

                        result += problemResult;
                    }
                }
            }
        }

        return result.ToString();
    }

    public override string Part2()
    {
        long result = 0;

        char[][] sheet = new char[4][];
        using (var input = File.OpenText(InputFile))
        {
            var y = 0;
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                for (var x = 0; x < line.Length; x++)
                {
                    if(x == 0) sheet[y] = new char[line.Length];
                    sheet[y][x] = line[x];
                }

                if (++y == 4)
                {
                    var x = 0;
                    var operators = input.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var op in operators)
                    {
                        long problemResult = 0;
                        if (op == "*") problemResult = 1;

                        while (true)
                        {
                            StringBuilder nextNum = new();
                            if(x == sheet[0].Length) break;
                            for (y = 0; y < 4; y++)
                            {
                                if (sheet[y][x] != ' ') nextNum.Append(sheet[y][x]);
                            }

                            if (nextNum.Length == 0)
                            {
                                x++;
                                break;
                            }

                            switch (op)
                            {
                                case "*": problemResult *= int.Parse(nextNum.ToString()); break;
                                case "+": problemResult += int.Parse(nextNum.ToString()); break;
                            }

                            x++;
                        }
                        result += problemResult;
                    }
                }
            }

            return result.ToString();
        }
    }
}