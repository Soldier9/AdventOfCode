using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode.Solvers.Year2019
{
    class Day23Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            readonly Dictionary<Int64, Int64> Program;
            Int64 IP;
            Int64 RelativeBase;

            readonly BlockingCollection<Int64> InputQueue;
            readonly BlockingCollection<Int64> OutputQueue;
            private readonly Queue<Int64> NicOutputQueue = new Queue<Int64>();
            public long failedNicReads = 0;

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

            public void RunProgram()
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
                            return;
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
                if (InputQueue.Count == 0)
                {
                    failedNicReads++;
                    Program[param1] = -1;
                }
                else
                {
                    failedNicReads = 0;
                    Program[param1] = InputQueue.Take();
                }
            }

            void OutputInstr(Int64[] modes)
            {
                Int64 param1 = GetParam(modes[0], 1);
                NicOutputQueue.Enqueue(param1);
                if (NicOutputQueue.Count == 3)
                {
                    while (NicOutputQueue.Count > 0) OutputQueue.Add(NicOutputQueue.Dequeue());
                }
                failedNicReads = 0;
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

            public static IEnumerable<Int64> GetIntCodeInput(string input)
            {
                foreach (var c in input)
                {
                    yield return Convert.ToInt64(c);
                }
            }
        }

        struct NIC
        {
            public int Address;
            public IntcodeCPU CPU;
            public Thread CPUThread;
            public BlockingCollection<Int64> OutputQueue;
            public BlockingCollection<Int64> InputQueue;

            public NIC(int address, IntcodeCPU cpu, Thread cpuThread, BlockingCollection<long> outputQueue, BlockingCollection<long> inputQueue)
            {
                Address = address;
                CPU = cpu;
                CPUThread = cpuThread;
                OutputQueue = outputQueue;
                InputQueue = inputQueue;
            }
        }

        public override string Part1()
        {
            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }

            var theSwitch = new List<NIC>();

            for (var i = 0; i < 50; i++)
            {
                var outputQueue = new BlockingCollection<Int64>();
                var inputQueue = new BlockingCollection<Int64>
                {
                    i
                };
                var cpu = new IntcodeCPU(program, inputQueue, outputQueue);
                var cpuThread = new Thread(new ThreadStart(cpu.RunProgram));
                cpuThread.Start();
                theSwitch.Add(new NIC(i, cpu, cpuThread, outputQueue, inputQueue));
            }

            while (true)
            {
                foreach (var nic in theSwitch)
                {
                    if (nic.OutputQueue.Count > 2)
                    {
                        var address = Convert.ToInt32(nic.OutputQueue.Take());
                        var x = nic.OutputQueue.Take();
                        var y = nic.OutputQueue.Take();
                        if (address == 255)
                        {
                            foreach (var nic2 in theSwitch) nic2.CPUThread.Abort();
                            return y.ToString();
                        }
                        theSwitch[address].InputQueue.Add(x);
                        theSwitch[address].InputQueue.Add(y);

                        Console.WriteLine("{0} -> {1}: {2}, {3}", nic.Address, address, x, y);
                    }
                }
            }
        }

        public override string Part2()
        {
            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }

            var theSwitch = new List<NIC>();

            for (var i = 0; i < 50; i++)
            {
                var outputQueue = new BlockingCollection<Int64>();
                var inputQueue = new BlockingCollection<Int64>
                {
                    i
                };
                var cpu = new IntcodeCPU(program, inputQueue, outputQueue);
                var cpuThread = new Thread(new ThreadStart(cpu.RunProgram));
                cpuThread.Start();
                theSwitch.Add(new NIC(i, cpu, cpuThread, outputQueue, inputQueue));
            }

            Int64 NATx = 0;
            Int64 NATy = 0;
            Int64 lastSentNATy = -1;

            while (true)
            {
                bool allIdle = true;
                foreach (var nic in theSwitch)
                {
                    if (nic.OutputQueue.Count > 2)
                    {
                        var address = Convert.ToInt32(nic.OutputQueue.Take());
                        var x = nic.OutputQueue.Take();
                        var y = nic.OutputQueue.Take();
                        if (address == 255)
                        {
                            NATx = x;
                            NATy = y;
                        }
                        else
                        {
                            theSwitch[address].InputQueue.Add(x);
                            theSwitch[address].InputQueue.Add(y);
                        }
                        Console.WriteLine("{0} -> {1}: {2}, {3}", nic.Address, address, x, y);
                    }
                    if (nic.CPU.failedNicReads < 100000) allIdle = false;
                }
                if (allIdle)
                {
                    theSwitch[0].InputQueue.Add(NATx);
                    theSwitch[0].InputQueue.Add(NATy);
                    theSwitch[0].CPU.failedNicReads = 0;
                    Console.WriteLine("{0} -> {1}: {2}, {3}", 255, 0, NATx, NATy);
                    if (lastSentNATy == NATy)
                    {
                        foreach (var nic in theSwitch) nic.CPUThread.Abort();
                        return NATy.ToString();
                    }
                    lastSentNATy = NATy;
                }
            }
        }
    }
}
