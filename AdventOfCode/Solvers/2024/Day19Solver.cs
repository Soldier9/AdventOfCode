namespace AdventOfCode.Solvers.Year2024
{
    class Day19Solver : AbstractSolver
    {
        List<string> Towels = [];
        List<string> Patterns = [];
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Towels = input.ReadLine()!.Split(',', StringSplitOptions.TrimEntries).ToList();
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Length == 0) continue;
                    Patterns.Add(line);
                }
            }

            int result = 0;
            foreach(string pattern in Patterns) if(ConstructPattern(pattern) > 0) result++;
            return result.ToString();
        }

        Dictionary<string, long> CachedResults = [];
        public long ConstructPattern(string pattern)
        {
            if (pattern == "") return 1;
            if (CachedResults.TryGetValue(pattern, out long result)) return result;
            foreach (string towel in Towels)
            {
                if (pattern.StartsWith(towel)) result += ConstructPattern(pattern[towel.Length..]);
            }
            CachedResults.Add(pattern, result);
            return result;
        }

        public override string Part2()
        {
            long result = 0;
            foreach (string pattern in Patterns) result += ConstructPattern(pattern);
            return result.ToString();
        }
    }
}
