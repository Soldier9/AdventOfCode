using System.Collections.Generic;

namespace AdventOfCode.Solvers.Year2022
{
    class Day4Solver : AbstractSolver
    {
        List<List<HashSet<int>>> pairs = new();

        public override string Part1()
        {
            int result = 0;
            
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    List<HashSet<int>> pair = new();
                    foreach(string elf in input.ReadLine()!.Split(","))
                    {
                        HashSet<int> sections = new HashSet<int>();
                        for(int i = int.Parse(elf.Split("-")[0]); i <= int.Parse(elf.Split("-")[1]); i++)
                        {
                            sections.Add(i);
                        }
                        pair.Add(sections);
                    }
                    pairs.Add(pair);
                    if (pair[0].IsSupersetOf(pair[1]) || pair[1].IsSupersetOf(pair[0])) result++;
                }
            }

            return result.ToString();

        }

        public override string Part2()
        {
            int result = 0;

            foreach(List<HashSet<int>> pair in pairs)
            {
                foreach(int section in pair[0])
                {
                    if (pair[1].Contains(section))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result.ToString();
        }
    }
}
