namespace AdventOfCode.Solvers.Year2024
{
    class Day5Solver : AbstractSolver
    {
        Dictionary<int, HashSet<int>> PreceedingPages = [];
        List<List<int>> AcceptedPrintJobs = [];
        List<List<int>> RejectedPrintJobs = [];

        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Contains('|'))
                    {
                        int[] pages = line.Split('|').Select(n => int.Parse(n)).ToArray();
                        if (!PreceedingPages.ContainsKey(pages[1])) PreceedingPages.Add(pages[1], new());
                        PreceedingPages[pages[1]].Add(pages[0]);
                    }
                    else if (line.Contains(','))
                    {
                        List<int> pages = line.Split(',').Select(n => int.Parse(n)).ToList();
                        for (int i = 0; i < pages.Count; i++)
                        {
                            if (PreceedingPages.ContainsKey(pages[i]))
                            {
                                foreach(int preceedingPage in PreceedingPages[pages[i]])
                                {
                                    if(pages.IndexOf(preceedingPage) > i)
                                    {
                                        RejectedPrintJobs.Add(pages);
                                        pages = [];
                                        break;
                                    }
                                }
                            }
                        }
                        if (pages.Count > 0) AcceptedPrintJobs.Add(pages);
                    }
                }
            }

            foreach (List<int> acceptedPrintJob in AcceptedPrintJobs)
            {
                result += acceptedPrintJob[acceptedPrintJob.Count / 2];
            }

            return result.ToString();
        }

        class Comparer : IComparer<int>
        {
            Dictionary<int, HashSet<int>> PreceedingPages;
            public Comparer(Dictionary<int, HashSet<int>> preceedingPages) => this.PreceedingPages = preceedingPages;

            public int Compare(int x, int y)
            {
                if (PreceedingPages.ContainsKey(x) && PreceedingPages[x].Contains(y)) return 1;
                if (PreceedingPages.ContainsKey(y) && PreceedingPages[y].Contains(x)) return -1;
                return 0;
            }
        }

        public override string Part2()
        {
            int result = 0;

            foreach (List<int> rejectedPrintJob in RejectedPrintJobs)
            {
                rejectedPrintJob.Sort(new Comparer(PreceedingPages));
                result += rejectedPrintJob[rejectedPrintJob.Count / 2];
            }

            return result.ToString();
        }
    }
}
