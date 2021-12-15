using System.Collections.Concurrent;
using System.Drawing;

namespace AdventOfCode.Solvers.Year2019
{
    class Day11Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            //readonly Int64[] Program;
            readonly Dictionary<long, long> Program;
            long IP;
            long RelativeBase;

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
                            OutputQueue.Add(-1);
                            return -1;
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
                Program[param1] = InputQueue.Take();
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
        public override string Part1()
        {

            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }
            BlockingCollection<long> outputQueue = new();
            BlockingCollection<long> inputQueue = new();
            inputQueue.Add(0);

            Task<long> cpuTask = new(() =>
            {
                return (new IntcodeCPU(program, inputQueue, outputQueue)).RunProgram();
            });
            cpuTask.Start();

            Point currentPosition = new(0, 0);
            char currentDirection = 'U';
            Dictionary<Point, int> panels = new();
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                int nextColor = Convert.ToInt32(outputQueue.Take());
                if (nextColor == -1) break;
                panels[currentPosition] = nextColor;
                switch (currentDirection)
                {
                    case 'U': currentDirection = outputQueue.Take() == 0 ? 'L' : 'R'; break;
                    case 'R': currentDirection = outputQueue.Take() == 0 ? 'U' : 'D'; break;
                    case 'D': currentDirection = outputQueue.Take() == 0 ? 'R' : 'L'; break;
                    case 'L': currentDirection = outputQueue.Take() == 0 ? 'D' : 'U'; break;
                }
                switch (currentDirection)
                {
                    case 'U': currentPosition.Y++; break;
                    case 'R': currentPosition.X++; break;
                    case 'D': currentPosition.Y--; break;
                    case 'L': currentPosition.X--; break;
                }
                int nextPanelColor = panels.ContainsKey(currentPosition) ? panels[currentPosition] : 0;
                inputQueue.Add(nextPanelColor);
            }

            return panels.Count.ToString();
        }

        public override string Part2()
        {
            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }
            BlockingCollection<long> outputQueue = new();
            BlockingCollection<long> inputQueue = new();
            inputQueue.Add(1);

            Task<long> cpuTask = new(() =>
            {
                return (new IntcodeCPU(program, inputQueue, outputQueue)).RunProgram();
            });
            cpuTask.Start();

            Point currentPosition = new(0, 0);
            char currentDirection = 'U';
            Dictionary<Point, int> panels = new();
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                int nextColor = Convert.ToInt32(outputQueue.Take());
                if (nextColor == -1) break;
                panels[currentPosition] = nextColor;
                switch (currentDirection)
                {
                    case 'U': currentDirection = outputQueue.Take() == 0 ? 'L' : 'R'; break;
                    case 'R': currentDirection = outputQueue.Take() == 0 ? 'U' : 'D'; break;
                    case 'D': currentDirection = outputQueue.Take() == 0 ? 'R' : 'L'; break;
                    case 'L': currentDirection = outputQueue.Take() == 0 ? 'D' : 'U'; break;
                }
                switch (currentDirection)
                {
                    case 'U': currentPosition.Y--; break;
                    case 'R': currentPosition.X++; break;
                    case 'D': currentPosition.Y++; break;
                    case 'L': currentPosition.X--; break;
                }
                int nextPanelColor = panels.ContainsKey(currentPosition) ? panels[currentPosition] : 0;
                inputQueue.Add(nextPanelColor);
            }

            int minX = panels.Keys.OrderBy(k => k.X).First().X;
            int minY = panels.Keys.OrderBy(k => k.Y).First().Y;
            int maxY = panels.Keys.OrderByDescending(k => k.Y).First().Y;

            int cursorLineAfterOutput = maxY - minY + Console.CursorTop + 1;
            minY += Console.CursorTop;

            foreach (KeyValuePair<Point, int> panel in panels)
            {
                Console.SetCursorPosition(panel.Key.X + minX, panel.Key.Y + minY);
                switch (panel.Value)
                {
                    case 0: Console.Write(" "); break;
                    case 1: Console.Write("#"); break;

                }
            }

            Console.SetCursorPosition(0, cursorLineAfterOutput);
            return "Output from program";
        }
    }
}
