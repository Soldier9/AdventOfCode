using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day6Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;

            using (var input = File.OpenText(InputFile))
            {
                foreach (string group in input.ReadToEnd().Split(new[] { "\r\n\r\n" }, System.StringSplitOptions.None))
                {
                    HashSet<char> answers = new HashSet<char>();
                    foreach (string replies in group.Split(new[] { "\r\n" }, System.StringSplitOptions.None))
                    {
                        foreach (char c in replies)
                        {
                            answers.Add(c);
                        }
                    }

                    result += answers.Count;
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;

            using (var input = File.OpenText(InputFile))
            {

                foreach (string group in input.ReadToEnd().Split(new[] { "\r\n\r\n" }, System.StringSplitOptions.None))
                {
                    Dictionary<char, int> answers = new Dictionary<char, int>();
                    int groupMembers = 0;

                    foreach (string replies in group.Split(new[] { "\r\n" }, System.StringSplitOptions.None))
                    {
                        groupMembers++;
                        foreach (char c in replies)
                        {
                            if (answers.ContainsKey(c)) answers[c]++;
                            else answers.Add(c, 1);
                        }
                    }

                    foreach (var answer in answers) if (answer.Value == groupMembers) result++;
                }
            }

            return result.ToString();
        }
    }
}
