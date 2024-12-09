namespace AdventOfCode.Solvers.Year2024
{
    class Day9Solver : AbstractSolver
    {
        public override string Part1()
        {
            long result = 0;
            List<int> disk = [];
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    bool file = true;
                    int fileId = 0;
                    foreach (char c in line)
                    {
                        for (int i = 0; i < int.Parse(c.ToString()); i++)
                        {
                            if (file) disk.Add(fileId);
                            else disk.Add(-1);
                        }
                        if (file) fileId++;
                        file = !file;
                    }
                }
            }

            bool defragDone = false;
            for (int i = disk.Count - 1; i >= 0; i--)
            {
                if (disk[i] > -1)
                {
                    for (int j = 0; j < disk.Count; j++)
                    {
                        if (disk[j] == -1)
                        {
                            disk[j] = disk[i];
                            disk[i] = -1;
                            break;
                        }
                        if (i == j)
                        {
                            defragDone = true;
                            break;
                        }
                    }
                    if (defragDone) break;
                }
            }

            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] > -1) result += i * disk[i];
            }

            return result.ToString();
        }

        public override string Part2()
        {
            long result = 0;
            List<int> disk = [];
            Dictionary<int, (int pos, int length)> fileMap = [];
            int fileId = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    bool file = true;

                    foreach (char c in line)
                    {
                        for (int i = 0; i < int.Parse(c.ToString()); i++)
                        {
                            if (file)
                            {
                                if (i == 0) fileMap.Add(fileId, (disk.Count, int.Parse(c.ToString())));
                                disk.Add(fileId);
                            }
                            else disk.Add(-1);
                        }
                        if (file) fileId++;
                        file = !file;
                    }
                }
            }

            if (!fileMap.ContainsKey(fileId)) fileId--;
            for (int f = fileId; f > -1; f--)
            {
                for (int i = 0; i < fileMap[f].pos; i++)
                {
                    if (disk[i] == -1)
                    {
                        bool fileFits = true;
                        for (int j = 0; j < fileMap[f].length; j++)
                        {
                            if (disk[i + j] > -1)
                            {
                                fileFits = false;
                                break;
                            }
                        }
                        if (fileFits)
                        {
                            for (int j = 0; j < fileMap[f].length; j++)
                            {
                                disk[i + j] = f;
                                disk[fileMap[f].pos + j] = -1;
                            }
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < disk.Count; i++)
            {
                if (disk[i] > -1) result += i * disk[i];
            }

            return result.ToString();
        }
    }
}
