using System.Collections.Concurrent;
using System.Drawing;

namespace AdventOfCode.Solvers.Year2019
{
    class Day15Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            readonly Dictionary<long, long> Program;
            long IP;
            long RelativeBase;
            public bool WaitingForInput = false;

            readonly BlockingCollection<long> InputQueue;
            readonly BlockingCollection<long> OutputQueue;

            public IntcodeCPU(long[] program, BlockingCollection<long> input, BlockingCollection<long> output)
            {
                IP = 0;
                Program = new Dictionary<long, long>();
                for (long i = 0; i < program.Length; i++) Program[i] = program[i];
                InputQueue = input;
                OutputQueue = output;
            }

            public void Input(long input)
            {
                InputQueue.Add(input);
            }

            public long RunProgram()
            {
                while (true)
                {
                    long opCode = Program[IP];

                    long[] modes = new long[3];
                    modes[0] = (opCode % 1000) / 100;
                    modes[1] = (opCode % 10000) / 1000;
                    modes[2] = (opCode % 100000) / 10000;
                    opCode %= 100;

                    switch (opCode)
                    {
                        case 1:
                            AddInstr(modes);
                            IP += 4;
                            break;
                        case 2:
                            MultInstr(modes);
                            IP += 4;
                            break;
                        case 3:
                            InputInstr(modes);
                            IP += 2;
                            break;
                        case 4:
                            OutputInstr(modes);
                            IP += 2;
                            break;
                        case 5:
                            JumpIfTrueInstr(modes);
                            break;
                        case 6:
                            JumpIfFalseInstr(modes);
                            break;
                        case 7:
                            LessThanInstr(modes);
                            IP += 4;
                            break;
                        case 8:
                            EqualsInstr(modes);
                            IP += 4;
                            break;
                        case 9:
                            ModifyRelativeBaseInstr(modes);
                            IP += 2;
                            break;


                        case 99:
                            OutputQueue.Add(-2);
                            return -2;
                    }
                }
            }

            void AddInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);
                long param3 = GetParam(modes[2], 3, true);

                Program[param3] = param1 + param2;
            }

            void MultInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);
                long param3 = GetParam(modes[2], 3, true);

                Program[param3] = param1 * param2;
            }

            void InputInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1, true);
                if (InputQueue.Count == 0) WaitingForInput = true;
                Program[param1] = InputQueue.Take();
                WaitingForInput = false;
            }

            void OutputInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                OutputQueue.Add(param1);
            }

            void JumpIfTrueInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);

                if (param1 != 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }
            void JumpIfFalseInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);

                if (param1 == 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }

            void LessThanInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);
                long param3 = GetParam(modes[2], 3, true);

                Program[param3] = (param1 < param2 ? 1 : 0);
            }

            void EqualsInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                long param2 = GetParam(modes[1], 2);
                long param3 = GetParam(modes[2], 3, true);

                Program[param3] = (param1 == param2 ? 1 : 0);
            }

            void ModifyRelativeBaseInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                RelativeBase += param1;
            }

            long GetParam(long mode, int paramNum, bool isTargetParam = false)
            {
                switch (mode)
                {
                    case 0: // Position mode
                        if (isTargetParam) return Program[IP + paramNum];
                        else if (Program.ContainsKey(Program[IP + paramNum])) return Program[Program[IP + paramNum]];
                        else return 0;

                    case 1: // Immediate mode
                        return Program[IP + paramNum];

                    case 2: // Relative mode
                        if (isTargetParam) return RelativeBase + Program[IP + paramNum];
                        else if (Program.ContainsKey(RelativeBase + Program[IP + paramNum])) return Program[RelativeBase + Program[IP + paramNum]];
                        else return 0;
                }
                throw new NotImplementedException();
            }
        }

        private class Tile
        {
            public static Dictionary<Point, Tile> AllTiles = new();
            public static Stack<Point> UnexploredNeighboors = new();

            public readonly Point Location;
            public readonly int Type;
            public int MovesFromStart = 0;

            public Tile? PrevTile;
            public readonly List<Point> Neighboors = new();

            public Tile(Point location, int type, Tile? predecessor)
            {
                Location = location;
                Type = type;
                PrevTile = predecessor;
                MovesFromStart = (PrevTile == null) ? 0 : PrevTile.MovesFromStart + 1;

                Neighboors.Add(new Point(Location.X, Location.Y - 1));
                Neighboors.Add(new Point(Location.X, Location.Y + 1));
                Neighboors.Add(new Point(Location.X - 1, Location.Y));
                Neighboors.Add(new Point(Location.X + 1, Location.Y));
                foreach (Point unexploredNeighboor in Neighboors)
                {
                    if (!AllTiles.ContainsKey(unexploredNeighboor)) UnexploredNeighboors.Push(unexploredNeighboor);
                    else if (AllTiles[unexploredNeighboor].MovesFromStart > MovesFromStart + 1) AllTiles[unexploredNeighboor].SetBetterPrevTile(this);
                }
            }


            public void Print(int xOffset, int yOffset, string type)
            {
                Console.SetCursorPosition(Location.X + xOffset, Location.Y + yOffset);
                Console.Write(type);
            }


            public void SetBetterPrevTile(Tile betterPrevTile)
            {
                PrevTile = betterPrevTile;
                MovesFromStart = betterPrevTile.MovesFromStart + 1;
                foreach (Point neighboor in Neighboors)
                {
                    if (AllTiles.ContainsKey(neighboor) && AllTiles[neighboor].MovesFromStart > MovesFromStart - 1) AllTiles[neighboor].SetBetterPrevTile(this);
                }
            }

            public int FindDirectionTo(Point neighboor)
            {
                if (Location.X == neighboor.X && Location.Y == neighboor.Y - 1) return 1;
                if (Location.X == neighboor.X && Location.Y == neighboor.Y + 1) return 2;
                if (Location.X == neighboor.X - 1 && Location.Y == neighboor.Y) return 3;
                if (Location.X == neighboor.X + 1 && Location.Y == neighboor.Y) return 4;
                return 0;
            }

            public override string ToString()
            {
                return "Location: " + Location.ToString() + ", Type: " + Type.ToString();
            }
        }

        public override string Part1()
        {

            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }
            BlockingCollection<long> outputQueue = new();
            BlockingCollection<long> inputQueue = new();

            IntcodeCPU cpu = new(program, inputQueue, outputQueue);
            Task<long> cpuTask = new(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            Tile currentTile = new(new Point(0, 0), 1, null);
            Tile.AllTiles.Add(currentTile.Location, currentTile);
            while (Tile.UnexploredNeighboors.Count > 0)
            {
                Point nextLocation = Tile.UnexploredNeighboors.Pop();
                int nextDirection = currentTile.FindDirectionTo(nextLocation);
                if (nextDirection == 0)
                {
                    do
                    {
                        inputQueue.Add(currentTile.FindDirectionTo(currentTile.PrevTile!.Location));
                        currentTile = currentTile.PrevTile;
                        _ = outputQueue.Take(); // eat already known contents
                        nextDirection = currentTile.FindDirectionTo(nextLocation);
                    } while (nextDirection == 0);
                }

                inputQueue.Add(nextDirection);
                int nextType = Convert.ToInt32(outputQueue.Take());
                if (nextType == 0) continue; // we hit a wall and did not move

                currentTile = new Tile(nextLocation, nextType, currentTile);
                Tile.AllTiles.Add(currentTile.Location, currentTile);
            }

            return Tile.AllTiles.Single(t => t.Value.Type == 2).Value.MovesFromStart.ToString();
        }

        public override string Part2()
        {
            foreach (Tile tile in Tile.AllTiles.Values)
            {
                tile.MovesFromStart = int.MaxValue;
                tile.PrevTile = null;
            }

            Tile oxygenTile = Tile.AllTiles.Single(t => t.Value.Type == 2).Value;
            oxygenTile.MovesFromStart = 0;
            foreach (Point neighboorLocation in oxygenTile.Neighboors)
            {
                if (Tile.AllTiles.ContainsKey(neighboorLocation)) Tile.AllTiles[neighboorLocation].SetBetterPrevTile(oxygenTile);
            }

            return Tile.AllTiles.OrderByDescending(t => t.Value.MovesFromStart).First().Value.MovesFromStart.ToString();
        }
    }
}
