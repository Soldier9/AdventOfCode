namespace AdventOfCode.Solvers.Year2022
{
    class Day7Solver : AbstractSolver
    {
        interface ElfFsEntry
        {
            string Name { get; }
            int Size { get; }
        }

        class ElfDir : ElfFsEntry
        {
            string name;
            public string Name => name;

            public ElfDir? parent;
            public int Size => this.Content.Sum(e => e.Size);

            public List<ElfFsEntry> Content = new();

            public ElfDir(string name, ElfDir? parent)
            {
                this.name = name;
                this.parent = parent;
            }

            public int getSumOfMaxSize(int maxSize)
            {
                int result = 0;

                foreach (ElfDir e in this.Content.Where(e => e is ElfDir))
                {
                    if (e.Size <= maxSize) result += e.Size;
                    result += e.getSumOfMaxSize(maxSize);
                }

                return result;
            }

            public ElfDir? smallestAboveSize(int reqSize)
            {
                ElfDir? result = null;
                foreach (ElfDir e in this.Content.Where(e => e is ElfDir))
                {
                    if (e.Size >= reqSize && (result is null || e.Size < result.Size))
                    {
                        result = e;
                    }

                    ElfDir? otherCandidate = e.smallestAboveSize(reqSize);
                    if (otherCandidate is not null && otherCandidate.Size >= reqSize && (result is null || otherCandidate.Size < result.Size))
                    {
                        result = otherCandidate;
                    }
                }
                return result!;
            }

        }

        class ElfFile : ElfFsEntry
        {
            string name;
            public string Name => name;

            int size;
            public int Size => size;

            public ElfFile(string name, int size)
            {
                this.name = name;
                this.size = size;
            }
        }

        List<ElfFsEntry> fileSystem = new();

        public override string Part1()
        {
            ElfDir pwd = new ElfDir("/", null);
            fileSystem.Add(pwd);

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Substring(0, 1) == "$")
                    {
                        if (line.Substring(2, 2) == "cd")
                        {
                            string name = line.Split(" ")[2];
                            if (name == "..") pwd = pwd.parent!;
                            else if (name == "/") pwd = (ElfDir)fileSystem[0];
                            else pwd = (ElfDir)pwd.Content.Single(e => e is ElfDir && e.Name == name);
                        }
                    }
                    else
                    {
                        string name = line.Split(" ")[1];
                        if (line.Split(" ")[0] == "dir")
                        {
                            if (!pwd.Content.Any(e => e is ElfDir && e.Name == name)) pwd.Content.Add(new ElfDir(name, pwd));
                        }
                        else
                        {
                            int size = int.Parse(line.Split(" ")[0]);
                            if (!pwd.Content.Any(e => e is ElfFile && e.Name == name)) pwd.Content.Add(new ElfFile(name, size));
                        }
                    }
                }
            }

            pwd = (ElfDir)fileSystem[0];
            return pwd.getSumOfMaxSize(100000).ToString();
        }

        public override string Part2()
        {
            ElfDir pwd = (ElfDir)fileSystem[0];

            int freeSpace = 70000000 - pwd.Size;
            int spaceNeeded = 30000000 - freeSpace;

            return pwd.smallestAboveSize(spaceNeeded)!.Size.ToString();
        }
    }
}
