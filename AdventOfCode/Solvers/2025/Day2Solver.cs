using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers._2025;

internal class Day2Solver : AbstractSolver
{
    private readonly List<(long min, long max)> _ranges = [];

    public override string Part1()
    {
        using (var input = File.OpenText(InputFile))
        {
            foreach (var range in input.ReadLine()!.Split(','))
            {
                var splitRange = range.Split('-');
                _ranges.Add((long.Parse(splitRange[0]), long.Parse(splitRange[1])));
            }
        }

        var testId = new Regex(@"^(\d+)\1$");
        var result = _ranges.Sum(range => FindInvalidIDs(range, testId).Sum());
        return result.ToString();
    }

    private List<long> FindInvalidIDs((long min, long max) range, Regex testRegex)
    {
        List<long> invalidIds = new();
        for (var i = range.min; i <= range.max; i++)
        {
            var stringId = i.ToString();
            if (testRegex.IsMatch(stringId)) invalidIds.Add(i);
        }

        return invalidIds;
    }

    public override string Part2()
    {
        var testId = new Regex(@"^(\d+)\1+$");
        var result = _ranges.Sum(range => FindInvalidIDs(range, testId).Sum());
        return result.ToString();
    }
}