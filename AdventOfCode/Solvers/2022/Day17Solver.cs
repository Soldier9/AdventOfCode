using System.Numerics;

namespace AdventOfCode.Solvers.Year2022
{
    class Day17Solver : AbstractSolver
    {
        public override bool HasVisualization => base.HasVisualization;

        string jets = "";
        public override string Part1()
        {
            List<List<(int x, int y)>> rockTypes = new();
            HashSet<(int x, int y)> map = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    jets = input.ReadLine()!;
                }
            }

            List<(int x, int y)> rock = new();
            rock.Add((0, 0)); rock.Add((1, 0)); rock.Add((2, 0)); rock.Add((3, 0));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((1, 0)); rock.Add((0, 1)); rock.Add((1, 1)); rock.Add((2, 1)); rock.Add((1, 2));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((2, 0)); rock.Add((2, 1)); rock.Add((0, 2)); rock.Add((1, 2)); rock.Add((2, 2));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((0, 0)); rock.Add((0, 1)); rock.Add((0, 2)); rock.Add((0, 3));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((0, 0)); rock.Add((1, 0)); rock.Add((0, 1)); rock.Add((1, 1));
            rockTypes.Add(rock);

            for (int x = 0; x < 7; x++) map.Add((x, 0));

            int jetNum = 0;
            for (int i = 0; i < 2022; i++)
            {
                rock = new(rockTypes[i % rockTypes.Count]);
                bool stillFalling = true;
                int height = map.MinBy(c => c.y).y;


                int rockHeight = rock.MaxBy(r => r.y).y + 1;
                for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x + 2, rock[j].y + height - 3 - rockHeight);

                while (stillFalling)
                {
                    bool canMoveHorizontally = true;
                    switch (jets[jetNum++ % jets.Length])
                    {
                        case '<':
                            if (rock.MinBy(r => r.x).x <= 0) break;
                            for (int j = 0; j < rock.Count; j++)
                            {
                                if (map.Contains((rock[j].x - 1, rock[j].y)))
                                {
                                    canMoveHorizontally = false;
                                    break;
                                }
                            }
                            if (!canMoveHorizontally) break;
                            for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x - 1, rock[j].y);
                            break;
                        case '>':
                            if (rock.MaxBy(r => r.x).x >= 6) break;
                            for (int j = 0; j < rock.Count; j++)
                            {
                                if (map.Contains((rock[j].x + 1, rock[j].y)))
                                {
                                    canMoveHorizontally = false;
                                    break;
                                }
                            }
                            if (!canMoveHorizontally) break;
                            for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x + 1, rock[j].y);
                            break;
                    }
                    for (int j = 0; j < rock.Count; j++)
                    {
                        if (map.Contains((rock[j].x, rock[j].y + 1)))
                        {
                            stillFalling = false;
                            break;
                        }
                    }
                    if (!stillFalling) break;
                    for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x, rock[j].y + 1);
                }
                for (int j = 0; j < rock.Count; j++) map.Add(rock[j]);

                if(Program.VisualizationEnabled) Program.PrintData(Program.CreateStringFromDict(map), 100, true, true);
            }


            return Math.Abs(map.MinBy(c => c.y).y).ToString();
        }

        public override string Part2()
        {
            List<List<(int x, BigInteger y)>> rockTypes = new();
            HashSet<(int x, BigInteger y)> map = new();


            List<(int x, BigInteger y)> rock = new();
            rock.Add((0, 0)); rock.Add((1, 0)); rock.Add((2, 0)); rock.Add((3, 0));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((1, 0)); rock.Add((0, 1)); rock.Add((1, 1)); rock.Add((2, 1)); rock.Add((1, 2));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((2, 0)); rock.Add((2, 1)); rock.Add((0, 2)); rock.Add((1, 2)); rock.Add((2, 2));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((0, 0)); rock.Add((0, 1)); rock.Add((0, 2)); rock.Add((0, 3));
            rockTypes.Add(rock);
            rock = new();
            rock.Add((0, 0)); rock.Add((1, 0)); rock.Add((0, 1)); rock.Add((1, 1));
            rockTypes.Add(rock);

            for (int x = 0; x < 7; x++) map.Add((x, 0));
            int jetNum = 0;

            Dictionary<BigInteger, BigInteger> height2rock = new();
            height2rock.Add(0, -1);
            BigInteger heightOffset = 0;
            BigInteger cycles = 0;

            for (BigInteger i = 0; i < 1000000000000; i++)
            {
                BigInteger height = map.MinBy(c => c.y).y;
                for (BigInteger h = height2rock.MinBy(n => n.Key).Key - 1; h >= height; h--)
                {
                    if (height2rock.ContainsKey(h)) height2rock[h] = i; else height2rock.Add(h, i);
                }
                

                if (heightOffset == 0 && height * -1 > jets.Length)
                {
                    for (BigInteger c = height + 20 + 1; c < 0; c++)
                    {
                        bool cycleFound = true;
                        for (int cycleLength = 0; cycleLength < 20; cycleLength++)
                        {
                            for (int x = 0; x < 7; x++)
                            {
                                if ((map.Contains((x, c + cycleLength)) ^ map.Contains((x, height + 20 + cycleLength))))
                                {
                                    cycleFound = false;
                                    break;
                                }
                            }
                            if (!cycleFound) break;
                        }
                        if (cycleFound)
                        {
                            heightOffset = c - (height + 20);
                            BigInteger rockOffset = height2rock[height + 20] - height2rock[c];

                            cycles = ((1000000000000 - i) / rockOffset);
                            i += (cycles * rockOffset);
                            break;
                        }
                    }
                }

                bool stillFalling = true;
                rock = new(rockTypes[(int)(i % rockTypes.Count)]);
                BigInteger rockHeight = rock.MaxBy(r => r.y).y + 1;
                for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x + 2, rock[j].y + height - 3 - rockHeight);

                while (stillFalling)
                {
                    bool canMoveHorizontally = true;
                    switch (jets[jetNum++ % jets.Length])
                    {
                        case '<':
                            if (rock.MinBy(r => r.x).x <= 0) break;
                            for (int j = 0; j < rock.Count; j++)
                            {
                                if (map.Contains((rock[j].x - 1, rock[j].y)))
                                {
                                    canMoveHorizontally = false;
                                    break;
                                }
                            }
                            if (!canMoveHorizontally) break;
                            for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x - 1, rock[j].y);
                            break;
                        case '>':
                            if (rock.MaxBy(r => r.x).x >= 6) break;
                            for (int j = 0; j < rock.Count; j++)
                            {
                                if (map.Contains((rock[j].x + 1, rock[j].y)))
                                {
                                    canMoveHorizontally = false;
                                    break;
                                }
                            }
                            if (!canMoveHorizontally) break;
                            for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x + 1, rock[j].y);
                            break;
                    }
                    for (int j = 0; j < rock.Count; j++)
                    {
                        if (map.Contains((rock[j].x, rock[j].y + 1)))
                        {
                            stillFalling = false;
                            break;
                        }
                    }
                    if (!stillFalling) break;
                    for (int j = 0; j < rock.Count; j++) rock[j] = (rock[j].x, rock[j].y + 1);
                }
                for (int j = 0; j < rock.Count; j++) map.Add(rock[j]);
            }

            return (-1 * map.MinBy(c => c.y).y + (cycles * heightOffset)).ToString();
        }
    }
}
