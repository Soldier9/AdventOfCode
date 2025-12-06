namespace AdventOfCode.Solvers._2025;

internal class Day5Solver : AbstractSolver
{
    private readonly List<(long min, long max)> _ranges = new();
    private readonly List<long> _idsToCheck = new();

    public override string Part1()
    {
        var result = 0;

        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                var splitLine = line.Split("-", StringSplitOptions.RemoveEmptyEntries);
                if (splitLine.Length == 2)
                {
                    _ranges.Add((long.Parse(splitLine[0]), long.Parse(splitLine[1])));
                }
                else if (splitLine.Length == 1)
                {
                    _idsToCheck.Add(long.Parse(splitLine[0]));
                }
            }
        }

        foreach (var id in _idsToCheck)
        {
            foreach (var range in _ranges)
            {
                if (range.min <= id && id <= range.max)
                {
                    result++;
                    break;
                }
            }
        }

        return result.ToString();
    }

    public override string Part2()
    {
        var mergedRanges = _ranges;

        int preMergeCount;
        do
        {
            preMergeCount = mergedRanges.Count;
            mergedRanges = MergeRanges(mergedRanges);
        } while (mergedRanges.Count != preMergeCount);

        long result = 0;
        foreach (var range in mergedRanges) result += range.max - range.min + 1;
        return result.ToString();
    }


    private List<(long min, long max)> MergeRanges(List<(long min, long max)> ranges)
    {
        List<(long min, long max)> combined = new();
        foreach (var range in ranges)
        {
            bool rangeMerged = false;
            for (var i = 0; i < combined.Count; i++)
            {
                if (range.min >= combined[i].min && range.min <= combined[i].max)
                {
                    // range starts within combined[i] 
                    combined[i] = (combined[i].min, Math.Max(range.max, combined[i].max));
                    rangeMerged = true;
                    break;
                }

                if (range.max <= combined[i].max && range.max >= combined[i].min)
                {
                    // range ends within combined[i]
                    combined[i] = (Math.Min(range.min, combined[i].min), combined[i].max);
                    rangeMerged = true;
                    break;
                }

                if (range.min <= combined[i].min && range.max >= combined[i].max)
                {
                    // range encompasses combined[i] completely
                    combined[i] = range;
                    rangeMerged = true;
                    break;
                }
            }

            if (!rangeMerged) combined.Add(range);
        }

        return combined;
    }
}