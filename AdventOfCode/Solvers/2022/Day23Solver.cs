namespace AdventOfCode.Solvers.Year2022
{
    class Day23Solver : AbstractSolver
    {
        public override bool HasVisualization => true;
        public override bool HasExtendedVisualization => true;
        class Elf
        {
            public static Dictionary<(int x, int y), Elf> map = new();
            public static Queue<string> moves = new();

            public (int x, int y) pos;
            (int x, int y)? nextMovePos;

            public Elf((int x, int y) pos)
            {
                this.pos = pos;
                map.Add(pos, this);

                if(moves.Count == 0)
                {
                    moves.Enqueue("N");
                    moves.Enqueue("S");
                    moves.Enqueue("W");
                    moves.Enqueue("E");
                }
            }

            public (int x, int y)? findMove()
            {
                if (!(map.ContainsKey((this.pos.x, this.pos.y - 1)) ||
                    map.ContainsKey((this.pos.x + 1, this.pos.y - 1)) ||
                    map.ContainsKey((this.pos.x - 1, this.pos.y - 1)) ||
                    map.ContainsKey((this.pos.x - 1, this.pos.y)) ||
                    map.ContainsKey((this.pos.x + 1, this.pos.y)) ||
                    map.ContainsKey((this.pos.x, this.pos.y + 1)) ||
                    map.ContainsKey((this.pos.x + 1, this.pos.y + 1)) ||
                    map.ContainsKey((this.pos.x - 1, this.pos.y + 1)))) return null;

                List<string> localMoves = new(moves);
                for (int i = 0; i < moves.Count; i++)
                {
                    string nextMoveDirection = localMoves[i];
                    switch (nextMoveDirection)
                    {
                        case "N":
                            if (!map.ContainsKey((this.pos.x, this.pos.y - 1)) &&
                                !map.ContainsKey((this.pos.x + 1, this.pos.y - 1)) &&
                                !map.ContainsKey((this.pos.x - 1, this.pos.y - 1)))
                            {
                                nextMovePos = (this.pos.x, this.pos.y - 1);
                            }
                            break;

                        case "S":
                            if (!map.ContainsKey((this.pos.x, this.pos.y + 1)) &&
                                !map.ContainsKey((this.pos.x + 1, this.pos.y + 1)) &&
                                !map.ContainsKey((this.pos.x - 1, this.pos.y + 1)))
                            {
                                nextMovePos = (this.pos.x, this.pos.y + 1);
                            }
                            break;

                        case "W":
                            if (!map.ContainsKey((this.pos.x - 1, this.pos.y)) &&
                                !map.ContainsKey((this.pos.x - 1, this.pos.y - 1)) &&
                                !map.ContainsKey((this.pos.x - 1, this.pos.y + 1)))
                            {
                                nextMovePos = (this.pos.x - 1, this.pos.y);
                            }
                            break;

                        case "E":
                            if (!map.ContainsKey((this.pos.x + 1, this.pos.y)) &&
                                !map.ContainsKey((this.pos.x + 1, this.pos.y - 1)) &&
                                !map.ContainsKey((this.pos.x + 1, this.pos.y + 1)))
                            {
                                nextMovePos = (this.pos.x + 1, this.pos.y);
                            }
                            break;
                    }
                    if(nextMovePos != null) return nextMovePos;
                }
                return this.pos;
            }

            public void ValidateMoves()
            {
                List<Elf> sameMove = map.Values.Where(e => e.nextMovePos == this.nextMovePos && e != this).ToList<Elf>();
                foreach(Elf elf in sameMove)
                {
                    elf.nextMovePos = null;
                    this.nextMovePos = null;
                }
            }

            public static void DoMoves()
            {
                List<Elf> elvesToMove = map.Values.Where(e => e.nextMovePos != null).ToList<Elf>();
                foreach (Elf elf in elvesToMove)
                {
                    _ = map.Remove(elf.pos);
                    elf.pos = ((int x, int y))elf.nextMovePos!;
                    elf.nextMovePos = null;
                    map.Add(elf.pos, elf);
                }
                moves.Enqueue(moves.Dequeue());
            }
        }


        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach(char c in line)
                    {
                        if (c == '#') new Elf((x, y));
                        x++;
                    }
                    y++;
                }
            }

            if(Program.VisualizationEnabled) Program.PrintData(Program.CreateStringFromDict(Elf.map.Keys.ToHashSet()), 0, true, true);
            for (int i = 0; i < 10; i++) {
                bool keepMoving = false;
                foreach (Elf elf in Elf.map.Values) if(elf.findMove() != null) keepMoving = true;
                if (!keepMoving) break;
                foreach (Elf elf in Elf.map.Values) elf.ValidateMoves();
                Elf.DoMoves();
                if(Program.VisualizationEnabled) Program.PrintData(Program.CreateStringFromDict(Elf.map.Keys.ToHashSet()), 0, true, true);
            }

            (int x, int y) min = (Elf.map.MinBy(e => e.Key.x).Key.x, Elf.map.MinBy(e => e.Key.y).Key.y);
            (int x, int y) max = (Elf.map.MaxBy(e => e.Key.x).Key.x, Elf.map.MaxBy(e => e.Key.y).Key.y);
            int result = (max.x - min.x + 1) * (max.y - min.y + 1) - Elf.map.Count();
            return result.ToString();
        }


        public override string Part2()
        {
            Elf.map.Clear();
            Elf.moves.Clear();
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach (char c in line)
                    {
                        if (c == '#') new Elf((x, y));
                        x++;
                    }
                    y++;
                }
            }

            if(Program.ExtendedVisualization) Program.PrintData(Program.CreateStringFromDict(Elf.map.Keys.ToHashSet()), 0, true, true);
            int result = 0;
            while(true)
            {
                result++;
                bool keepMoving = false;
                foreach (Elf elf in Elf.map.Values) if (elf.findMove() != null) keepMoving = true;
                if (!keepMoving) return result.ToString();
                foreach (Elf elf in Elf.map.Values) elf.ValidateMoves();
                Elf.DoMoves();
                if (Program.ExtendedVisualization) Program.PrintData(Program.CreateStringFromDict(Elf.map.Keys.ToHashSet()), 0, true, true);
            }
        }
    }
}
