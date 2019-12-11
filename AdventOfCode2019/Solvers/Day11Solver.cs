using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day11Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            //readonly Int64[] Program;
            readonly Dictionary<Int64, Int64> Program;
            Int64 IP;
            Int64 RelativeBase;

            readonly BlockingCollection<Int64> InputQueue;
            readonly BlockingCollection<Int64> OutputQueue;
            Int64? LastOutput;
            public bool HasTerminated = false;

            public IntcodeCPU(Int64[] program, BlockingCollection<Int64> input, BlockingCollection<Int64> output)
            {
                IP = 0;
                Program = new Dictionary<long, long>();
                for (Int64 i = 0; i < program.Length; i++) Program[i] = program[i];
                InputQueue = input;
                OutputQueue = output;
            }

            public void Input(Int64 input)
            {
                InputQueue.Add(input);
            }

            public Int64 RunProgram()
            {
                while (true)
                {
                    Int64 opCode = Program[IP];

                    Int64[] modes = new Int64[3];
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
                            HasTerminated = true;
                            OutputQueue.Add(-1);
                            return -1;
                    }
                }
            }

            void AddInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);
                Int64 param3 = GetParam(modes[2], 3, true);

                Program[param3] = param1 + param2;
            }

            void MultInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);
                Int64 param3 = GetParam(modes[2], 3, true);

                Program[param3] = param1 * param2;
            }

            void InputInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1, true);
                Program[param1] = InputQueue.Take();
            }

            void OutputInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                LastOutput = param1;
                OutputQueue.Add(param1);
            }

            void JumpIfTrueInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);

                if (param1 != 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }
            void JumpIfFalseInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);

                if (param1 == 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }

            void LessThanInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);
                Int64 param3 = GetParam(modes[2], 3, true);

                Program[param3] = (param1 < param2 ? 1 : 0);
            }

            void EqualsInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                Int64 param2 = GetParam(modes[1], 2);
                Int64 param3 = GetParam(modes[2], 3, true);

                Program[param3] = (param1 == param2 ? 1 : 0);
            }

            void ModifyRelativeBaseInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                RelativeBase += param1;
            }

            Int64 GetParam(Int64 mode, int paramNum, bool isTargetParam = false)
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

            public override string ToString()
            {
                return "IP: " + IP.ToString() + ", LastOutput: " + LastOutput + ", Halted: " + HasTerminated;
            }
        }
        public override string Part1()
        {

            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }
            BlockingCollection<Int64> outputQueue = new BlockingCollection<Int64>();
#pragma warning disable IDE0028 // Simplify collection initialization
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();
#pragma warning restore IDE0028 // Simplify collection initialization
            inputQueue.Add(0);

            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return (new IntcodeCPU(program, inputQueue, outputQueue)).RunProgram();
            });
            cpuTask.Start();

            Point currentPosition = new Point(0, 0);
            char currentDirection = 'U';
            Dictionary<Point, Int64> panels = new Dictionary<Point, Int64>();
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                Int64 nextColor = outputQueue.Take();
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
                Int64 nextPanelColor = panels.ContainsKey(currentPosition) ? panels[currentPosition] : 0;
                inputQueue.Add(nextPanelColor);
            }

            return panels.Count.ToString();
        }

        public override string Part2()
        {
            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }
            BlockingCollection<Int64> outputQueue = new BlockingCollection<Int64>();
#pragma warning disable IDE0028 // Simplify collection initialization
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();
#pragma warning restore IDE0028 // Simplify collection initialization
            inputQueue.Add(1);

            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return (new IntcodeCPU(program, inputQueue, outputQueue)).RunProgram();
            });
            cpuTask.Start();

            Point currentPosition = new Point(0, 0);
            char currentDirection = 'U';
            Dictionary<Point, Int64> panels = new Dictionary<Point, Int64>();
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                Int64 nextColor = outputQueue.Take();
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
                Int64 nextPanelColor = panels.ContainsKey(currentPosition) ? panels[currentPosition] : 0;
                inputQueue.Add(nextPanelColor);
            }

            int minX = panels.Keys.OrderBy(k => k.X).First().X;
            int minY = panels.Keys.OrderBy(k => k.Y).First().Y;

            foreach(KeyValuePair<Point, Int64> panel in panels)
            {
                Console.SetCursorPosition(panel.Key.X + minX, panel.Key.Y + minY);
                switch(panel.Value)
                {
                    case 0: Console.Write(" "); break;
                    case 1: Console.Write("#"); break;

                }
            }

            return "";
        }
    }
}
