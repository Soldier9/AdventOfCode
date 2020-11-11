using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solvers.Year2019
{
    class Day13Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            readonly Dictionary<Int64, Int64> Program;
            Int64 IP;
            Int64 RelativeBase;
            public bool WaitingForInput = false;

            readonly BlockingCollection<Int64> InputQueue;
            readonly BlockingCollection<Int64> OutputQueue;

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
                            OutputQueue.Add(-2);
                            return -2;
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
                if (InputQueue.Count == 0) WaitingForInput = true;
                Program[param1] = InputQueue.Take();
                WaitingForInput = false;
            }

            void OutputInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
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

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            int minY = Console.CursorTop;
            int maxY = 0;
            int blocks = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                int x = Convert.ToInt32(outputQueue.Take());
                if (x == -2) break;

                int y = Convert.ToInt32(outputQueue.Take()) + minY;
                int blockType = Convert.ToInt32(outputQueue.Take());

                Console.SetCursorPosition(x, y);
                switch (blockType)
                {
                    case 1: Console.Write("#"); break;
                    case 2: Console.Write("¤"); blocks++; break;
                    case 3: Console.Write("-"); break;
                    case 4: Console.Write("o"); break;
                }

                maxY = Math.Max(y, maxY);
            }

            Console.SetCursorPosition(0, maxY + 2);

            return blocks.ToString();
        }

        public override string Part2()
        {
            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }
            program[0] = 2;

            BlockingCollection<Int64> outputQueue = new BlockingCollection<Int64>();
#pragma warning disable IDE0028 // Simplify collection initialization
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();
#pragma warning restore IDE0028 // Simplify collection initialization
            inputQueue.Add(0);

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });

            Console.Write("Play yourself (Y/N)?");
            bool interactive = Console.ReadKey().Key == ConsoleKey.Y;
            
            int minY = Console.CursorTop + 3;
            int maxY = 0;
            int score = 0;

            int ballX = 0;
            int paddleX = 0;

            cpuTask.Start();
            if (!interactive)
            {
                Task.Factory.StartNew(() =>
                {
                    while (!cpuTask.IsCompleted)
                    {
                        lock (outputQueue)
                        {
                            if (inputQueue.Count == 0 && outputQueue.Count == 0 && ballX > 0 && paddleX > 0)
                            {
                                int nextInput = 0;
                                if (paddleX < ballX) nextInput = 1;
                                else if (paddleX > ballX) nextInput = -1;
                                else if (paddleX == ballX) nextInput = 0;

                                ballX = 0;
                                paddleX = 0;
                                inputQueue.Add(nextInput);
                            }
                        }
                    }
                });
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    while(!cpuTask.IsCompleted)
                    {
                        //lock(outputQueue)
                        //{
                            if (cpu.WaitingForInput && inputQueue.Count == 0 && outputQueue.Count == 0)
                            {
                                //Console.SetCursorPosition(0, maxY);
                                switch(Console.ReadKey().Key)
                                {
                                    case ConsoleKey.LeftArrow: inputQueue.Add(-1); break;
                                    case ConsoleKey.RightArrow: inputQueue.Add(1); break;
                                    default: inputQueue.Add(0); break;
                                }
                            }
                        //}
                    }
                });
            }

            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {

                int x = Convert.ToInt32(outputQueue.Take());
                lock (outputQueue)
                {
                    if (x == -2) break;
                    if (x == -1)
                    {
                        outputQueue.Take();
                        score = Convert.ToInt32(outputQueue.Take());
                        Console.SetCursorPosition(0, minY - 1);
                        Console.Write("Score: {0}", score);
                        continue;
                    }

                    int y = Convert.ToInt32(outputQueue.Take()) + minY;
                    int blockType = Convert.ToInt32(outputQueue.Take());

                    Console.SetCursorPosition(x, y);
                    switch (blockType)
                    {
                        case 0: Console.Write(" "); break;
                        case 1: Console.Write("#"); break;
                        case 2: Console.Write("+"); break;
                        case 3:
                            Console.Write("-");
                            paddleX = x;
                            break;
                        case 4:
                            Console.Write("o");
                            ballX = x;
                            break;
                    }

                    maxY = Math.Max(y, maxY);
                    if(interactive) Console.SetCursorPosition(0, maxY);
                }
            }

            Console.SetCursorPosition(0, maxY + 2);

            return score.ToString();
        }
    }
}
