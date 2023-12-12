using System.Text;

namespace AdventOfCode.Solvers.Year2023
{
    class Day12Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<(string pattern, List<int> requirements)> springs = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ');
                    (string pattern, List<int> requirements) spring = (line[0], new());
                    spring.requirements.AddRange(line[1].Split(',').Select(x => int.Parse(x)));
                    springs.Add(spring);
                }
            }

            long result = 0;
            foreach ((string pattern, List<int> requirements) spring in springs)
            {
                result += GetAcceptedPermutations(spring.pattern, spring.requirements);
            }

            return result.ToString();
        }

        public override string Part2()
        {
            List<(string pattern, List<int> requirements)> springs = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ');

                    (string pattern, List<int> requirements) spring = ("", new());
                    List<string> patterns = new();
                    for (int i = 0; i < 5; i++)
                    {
                        patterns.Add(line[0]);
                        spring.requirements.AddRange(line[1].Split(',').Select(x => int.Parse(x)));
                    }
                    StringBuilder sb = new();
                    sb.AppendJoin('?', patterns);
                    spring.pattern = sb.ToString();
                    springs.Add(spring);
                }
            }

            long result = 0;
            foreach ((string pattern, List<int> requirements) spring in springs)
            {
                result += GetAcceptedPermutations(spring.pattern, spring.requirements);
            }

            return result.ToString();
        }

        static Dictionary<(string, string), long> memo = new();
        public long GetAcceptedPermutations(string pattern, List<int> requirements)
        {
            StringBuilder requirementStringBuilder = new();
            requirementStringBuilder.AppendJoin(',', requirements);
            (string, string) memoKey = (pattern, requirementStringBuilder.ToString());
            if (memo.ContainsKey(memoKey)) return memo[memoKey];

            if (pattern.Replace(".", "").Length < requirements.Sum())
            {
                memo.Add(memoKey, 0);
                return 0;
            }
            if (requirements.Count > 0 && requirements[0] == 0)  // the first requirement is fulfilled, so we must not have any more #s at the start of the string
            {
                if (pattern.Length > 0)
                {
                    switch (pattern[0])
                    {
                        case '#':
                            memo.Add(memoKey, 0);
                            return 0;
                        case '.': break;
                        case '?':
                            memo.Add(memoKey, GetAcceptedPermutations('.' + pattern[1..], requirements));
                            return memo[memoKey];
                    }
                }
                requirements.RemoveAt(0);
            }
            if (requirements.Count == 0)
            {
                if (pattern.Contains('#')) // if there are # left in the pattern without anymore requirements, the permutation is not valid
                {
                    memo.Add(memoKey, 0);
                    return 0;
                }
                else 
                {
                    memo.Add(memoKey, 1); // the only accepted solution in this branch is replacing all ? with ., so we can just return 1
                    return 1;
                }
            }
            if (pattern.Length == 0)    // implicitly there are still requirements here, as we had returned earlier just above otherwise!
            {
                memo.Add(memoKey, 0);
                return 0;
            }

            long result = 0;
            switch (pattern[0])
            {
                case '.':
                    memo.Add(memoKey, GetAcceptedPermutations(pattern[1..], requirements));
                    return memo[memoKey];
                case '#':
                    if (pattern[..requirements[0]].Contains('.')) return 0;
                    List<int> recursiveRequirements = new(requirements);
                    recursiveRequirements[0] = 0;
                    memo.Add(memoKey, GetAcceptedPermutations(pattern[requirements[0]..], recursiveRequirements));
                    return memo[memoKey];
                case '?':
                    result += GetAcceptedPermutations('.' + pattern[1..], requirements);
                    result += GetAcceptedPermutations('#' + pattern[1..], requirements);
                    break;
            }
            memo.Add(memoKey, result);
            return result;
        }
    }
}
