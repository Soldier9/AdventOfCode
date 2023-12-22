namespace AdventOfCode.Solvers.Year2023
{
    class Day22Solver : AbstractSolver
    {
        List<HashSet<(int x, int y, int z)>> bricks = new();
        Dictionary<(int x, int y, int z), int> index = new();
        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split('~');
                    int[] start = line[0].Split(',').Select(v => int.Parse(v)).ToArray();
                    int[] end = line[1].Split(',').Select(v => int.Parse(v)).ToArray();

                    HashSet<(int x, int y, int z)> brick = new();
                    for (int x = start[0]; x <= end[0]; x++)
                    {
                        for (int y = start[1]; y <= end[1]; y++)
                        {
                            for (int z = start[2]; z <= end[2]; z++)
                            {
                                brick.Add((x, y, z));
                            }
                        }
                    }
                    bricks.Add(brick);
                }
            }

            bricks = bricks.OrderBy(b => b.Min(bl => bl.z)).ToList();
            for (int i = 0; i < bricks.Count; i++)
            {
                foreach (var block in bricks[i]) index.Add(block, i);
            }

            bool cannotFall = false;
            do
            {
                for (int i = 0; i < bricks.Count; i++)
                {
                    if (bricks[i].Min(b => b.z) == 1) continue;
                    cannotFall = false;
                    foreach (var block in bricks[i])
                    {
                        if (index.ContainsKey((block.x, block.y, block.z - 1)) && index[(block.x, block.y, block.z - 1)] != i)
                        {
                            cannotFall = true;
                            break;
                        }
                    }
                    if (!cannotFall)
                    {
                        HashSet<(int x, int y, int z)> newBrick = new();
                        foreach (var block in bricks[i])
                        {
                            newBrick.Add((block.x, block.y, block.z - 1));
                            index.Remove(block);
                            index.Add((block.x, block.y, block.z - 1), i);
                        }
                        bricks[i] = newBrick;
                        break;
                    }
                }
            } while (!cannotFall);

            int result = 0;
            for (int i = 0; i < bricks.Count; i++)
            {
                bool canBeDisintegrated = true;
                foreach (var block in bricks[i])
                {
                    if (index.ContainsKey((block.x, block.y, block.z + 1)) && index[(block.x, block.y, block.z + 1)] != i)
                    {
                        bool hasOtherSupport = false;
                        foreach (var otherBrickBlock in bricks[index[(block.x, block.y, block.z + 1)]])
                        {
                            if (index.ContainsKey((otherBrickBlock.x, otherBrickBlock.y, otherBrickBlock.z - 1))
                                && index[(otherBrickBlock.x, otherBrickBlock.y, otherBrickBlock.z - 1)] != i
                                && index[(otherBrickBlock.x, otherBrickBlock.y, otherBrickBlock.z - 1)] != index[(block.x, block.y, block.z + 1)])
                            {
                                hasOtherSupport = true;
                                break;
                            }
                        }
                        canBeDisintegrated = hasOtherSupport;
                        if (!canBeDisintegrated) break;
                    }
                }
                if (canBeDisintegrated) result++;
                else bricksCausingCollapse.Add(i);
            }

            return result.ToString();
        }

        List<int> bricksCausingCollapse = new();
        public override string Part2()
        {
            int result = 0;
            foreach (var i in bricksCausingCollapse)
            {
                result += FallingBricks(bricks.Where(b => b != bricks[i]).ToList());
            }

            return result.ToString();
        }

        int FallingBricks(List<HashSet<(int x, int y, int z)>> bricks)
        {
            HashSet<int> fallingBricks = new();
            Dictionary<(int x, int y, int z), int> index = new();
            for (int i = 0; i < bricks.Count; i++)
            {
                foreach (var block in bricks[i]) index.Add(block, i);
            }

            bool cannotFall = false;
            do
            {
                for (int i = 0; i < bricks.Count; i++)
                {
                    if (bricks[i].Min(b => b.z) == 1) continue;
                    cannotFall = false;
                    foreach (var block in bricks[i])
                    {
                        if (index.ContainsKey((block.x, block.y, block.z - 1)) && index[(block.x, block.y, block.z - 1)] != i)
                        {
                            cannotFall = true;
                            break;
                        }
                    }
                    if (!cannotFall)
                    {
                        fallingBricks.Add(i);
                        HashSet<(int x, int y, int z)> newBrick = new();
                        foreach (var block in bricks[i])
                        {
                            newBrick.Add((block.x, block.y, block.z - 1));
                            index.Remove(block);
                            index.Add((block.x, block.y, block.z - 1), i);
                        }
                        bricks[i] = newBrick;
                        break;
                    }
                }
            } while (!cannotFall);

            return fallingBricks.Count;
        }
    }
}
