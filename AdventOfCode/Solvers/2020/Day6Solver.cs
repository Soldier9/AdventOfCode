namespace AdventOfCode.Solvers.Year2020
{
    class Day6Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                foreach (string group in input.ReadToEnd().Split(new[] { "\r\n\r\n" }, System.StringSplitOptions.None))
                {
                    HashSet<char> answers = new();
                    foreach (string replies in group.Split(new[] { "\r\n" }, System.StringSplitOptions.None))
                    {
                        foreach (char c in replies)
                        {
                            _ = answers.Add(c);
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

            using (StreamReader input = File.OpenText(InputFile))
            {

                foreach (string group in input.ReadToEnd().Split(new[] { "\r\n\r\n" }, System.StringSplitOptions.None))
                {
                    Dictionary<char, int> answers = new();
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

                    foreach (KeyValuePair<char, int> answer in answers) if (answer.Value == groupMembers) result++;
                }
            }

            return result.ToString();
        }
    }
}
