using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2019.Solvers
{
    class Day4SolverRegex : AbstractSolver
    {
        int InputFrom = 136818;
        int InputTo = 685979;

        bool IsValidPw(string testPw, bool part2 = false)
        {
            for (int i = 1; i < testPw.Length; i++)
            {
                if (testPw[i - 1] > testPw[i]) return false;
            }

            var matches = Regex.Matches(testPw, @"(\d)\1+").Cast<Match>();
            if (!part2 && matches.Any(m => m.Length >= 2)) return true;
            else if(part2 && matches.Any(m => m.Length == 2)) return true;

            return false;
        }

        public override string Part1()
        {
            int validPws = 0;
            for (int testPw = InputFrom; testPw <= InputTo; testPw++)
            {
                if (IsValidPw(testPw.ToString())) validPws++;
            }

            return validPws.ToString();
        }

        public override string Part2()
        {
            int validPws = 0;
            for (int testPw = InputFrom; testPw <= InputTo; testPw++)
            {
                if (IsValidPw(testPw.ToString(), true)) validPws++;
            }

            return validPws.ToString();
        }
    }
}
