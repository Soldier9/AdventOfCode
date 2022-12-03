namespace AdventOfCode.Solvers.Year2022
{
    class Day3Solver : AbstractSolver
    {
        readonly List<string> rucksacks = new();
        readonly Dictionary<char, int> prios = new();

        public override string Part1()
        {
            for (int i = 1; i < 27; i++)
            {
                prios.Add((char)(i + 96), i);
                prios.Add((char)(i + 64), i + 26);
            }

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    rucksacks.Add(input.ReadLine()!);
                }
            }

            int result = 0;
            foreach (string rucksack in rucksacks)
            {
                string comp1 = rucksack[..(rucksack.Length / 2)];
                string comp2 = rucksack[comp1.Length..];

                foreach (char c in comp1)
                {
                    if (comp2.Contains(c))
                    {
                        result += prios[c];
                        break;
                    }
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;
            for (int i = 0; i < rucksacks.Count; i++)
            {
                if (i % 3 == 2)
                {
                    foreach (char c in rucksacks[i])
                    {
                        if (rucksacks[i - 1].Contains(c) && rucksacks[i - 2].Contains(c))
                        {
                            result += prios[c];
                            break;
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
