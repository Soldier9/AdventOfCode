namespace AdventOfCode.Solvers._2025;

internal class Day1Solver : AbstractSolver
{
    public override string Part1()
    {
        var result = 0;

        var dialPosition = 50;
        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                switch (line[0])
                {
                    case 'L':
                        dialPosition -= Int32.Parse(line[1..]);
                        break;
                    case 'R':
                        dialPosition += Int32.Parse(line[1..]);
                        break;
                }

                if (dialPosition % 100 == 0) result++;
            }
        }

        return result.ToString();
    }

    public override string Part2()
    {
        var result = 0;

        var dialPosition = 50;
        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                var clicks = int.Parse(line[1..]);

                switch (line[0])
                {
                    case 'L':
                        if (clicks >= dialPosition)
                        {
                            if(dialPosition > 0) result++;
                            result += (clicks - dialPosition) / 100;
                        }
                        dialPosition -= clicks;
                        break;
                    case 'R':
                        if (clicks + dialPosition >= 100)
                        {
                            result += (clicks + dialPosition) / 100;
                        }
                        dialPosition += clicks;
                        break;
                }
                dialPosition %= 100;
                if(dialPosition < 0) dialPosition += 100;
            }
        }

        return result.ToString();
    }
}