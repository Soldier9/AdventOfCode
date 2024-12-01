using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solvers.Year2024
{
    class Day1Solver : AbstractSolver
    {
        List<int> list1 = [];
        List<int> list2 = [];

        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    list1.Add(int.Parse(line[0]));
                    list2.Add(int.Parse(line[1]));
                }
            }

            list1.Sort();
            list2.Sort();

            for (int i = 0; i < list1.Count; i++)
            {
                result += Math.Abs(list1[i] - list2[i]);
            }

            return result.ToString();
        }

        public override string Part2()
        {
            Dictionary<int, int> numberOfOccurences = [];

            int result = 0;
            foreach (int i in list2)
            {
                if (numberOfOccurences.ContainsKey(i)) numberOfOccurences[i]++;
                else numberOfOccurences.Add(i, 1);
            }

            foreach (int i in list1) if (numberOfOccurences.TryGetValue(i, out int count)) result += i * count;
            return result.ToString();
        }
    }
}
