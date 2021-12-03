using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2021
{
    class Day1Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;
            using(var input = File.OpenText(InputFile))
            {
                int oldMeasure = int.MaxValue;
                while (!input.EndOfStream)
                {
                    int newMeasure = int.Parse(input.ReadLine());
                    if (newMeasure > oldMeasure) result++;
                    oldMeasure = newMeasure;
                }
            }
            
            return result.ToString();
        }

        public override string Part2()
        {
            List<int> measures = new List<int>();
            using (var input = File.OpenText(InputFile))
            {
                
                while (!input.EndOfStream)
                {
                    measures.Add(int.Parse(input.ReadLine()));
                }
            }

            int result = 0;
            int measure = 0;
            int prevMeasure = int.MaxValue;
            for (int i = 0; i < measures.Count; i++)
            {
                measure += measures[i];
                if (i > 1)
                {
                    if (i > 2) measure -= measures[i - 3];
                    if (measure > prevMeasure) result++;
                    prevMeasure = measure;
                }
            }

            return result.ToString();
        }
    }
}
