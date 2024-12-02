namespace AdventOfCode.Solvers.Year2024
{
    class Day2Solver : AbstractSolver
    {
        List<List<int>> reports = [];

        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    reports.Add([]);
                    foreach (string value in input.ReadLine()!.Split(' '))
                    {
                        reports[^1].Add(int.Parse(value));
                    }
                }
            }

            foreach (List<int> report in reports)
            {
                List<int> differences = report.Where((val, idx) => idx > 0).Select((val, idx) => val - report[idx]).ToList();
                if (differences.All(diff => (diff > 0 && diff <= 3)) ||
                    differences.All(diff => (diff < 0 && diff >= -3))) result++;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;

            foreach (List<int> report in reports)
            {
                List<int> differences = report.Where((val, idx) => idx > 0).Select((val, idx) => val - report[idx]).ToList();
                if (differences.All(diff => (diff > 0 && diff <= 3)) ||
                    differences.All(diff => (diff < 0 && diff >= -3))) result++;
                else
                {
                    for (int i = 0; i < report.Count; i++)
                    {
                        List<int> possibleReport = new(report);
                        possibleReport.RemoveAt(i);

                        differences = possibleReport.Where((val, idx) => idx > 0).Select((val, idx) => val - possibleReport[idx]).ToList();
                        if (differences.All(diff => (diff > 0 && diff <= 3)) ||
                            differences.All(diff => (diff < 0 && diff >= -3)))
                        {
                            result++;
                            break;
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
