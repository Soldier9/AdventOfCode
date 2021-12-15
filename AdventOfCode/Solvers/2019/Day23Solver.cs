using System.Collections.Concurrent;

namespace AdventOfCode.Solvers.Year2019
{
    class Day23Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            readonly Dictionary<long, long> Program;
            long IP;
            long RelativeBase;

            readonly BlockingCollection<long> InputQueue;
            readonly BlockingCollection<long> OutputQueue;
            private readonly Queue<long> NicOutputQueue = new();
            public long failedNicReads = 0;

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

            public void RunProgram()
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
                            return;
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

            void OutputInstr(long[] modes)
            {
                long param1 = GetParam(modes[0], 1);
                NicOutputQueue.Enqueue(param1);
                if (NicOutputQueue.Count == 3)
                {
                    while (NicOutputQueue.Count > 0) OutputQueue.Add(NicOutputQueue.Dequeue());
                }
                failedNicReads = 0;
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

            public static IEnumerable<long> GetIntCodeInput(string input)
            {
                foreach (char c in input)
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
            public BlockingCollection<long> OutputQueue;
            public BlockingCollection<long> InputQueue;

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
            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }

            List<NIC> theSwitch = new();

            for (int i = 0; i < 50; i++)
            {
                BlockingCollection<long> outputQueue = new();
                BlockingCollection<long> inputQueue = new()
                {
                    i
                };
                IntcodeCPU cpu = new(program, inputQueue, outputQueue);
                Thread cpuThread = new(new ThreadStart(cpu.RunProgram));
                cpuThread.Start();
                theSwitch.Add(new NIC(i, cpu, cpuThread, outputQueue, inputQueue));
            }

            while (true)
            {
                foreach (NIC nic in theSwitch)
                {
                    if (nic.OutputQueue.Count > 2)
                    {
                        int address = Convert.ToInt32(nic.OutputQueue.Take());
                        long x = nic.OutputQueue.Take();
                        long y = nic.OutputQueue.Take();
                        if (address == 255)
                        {
                            foreach (NIC nic2 in theSwitch) throw new PlatformNotSupportedException(); //nic2.CPUThread.Abort(); FIXTHIS
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
            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }

            List<NIC> theSwitch = new();

            for (int i = 0; i < 50; i++)
            {
                BlockingCollection<long> outputQueue = new();
                BlockingCollection<long> inputQueue = new()
                {
                    i
                };
                IntcodeCPU cpu = new(program, inputQueue, outputQueue);
                Thread cpuThread = new(new ThreadStart(cpu.RunProgram));
                cpuThread.Start();
                theSwitch.Add(new NIC(i, cpu, cpuThread, outputQueue, inputQueue));
            }

            long NATx = 0;
            long NATy = 0;
            long lastSentNATy = -1;

            while (true)
            {
                bool allIdle = true;
                foreach (NIC nic in theSwitch)
                {
                    if (nic.OutputQueue.Count > 2)
                    {
                        int address = Convert.ToInt32(nic.OutputQueue.Take());
                        long x = nic.OutputQueue.Take();
                        long y = nic.OutputQueue.Take();
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
                        foreach (NIC nic in theSwitch) throw new PlatformNotSupportedException(); //nic.CPUThread.Abort();
                        return NATy.ToString();
                    }
                    lastSentNATy = NATy;
                }
            }
        }
    }
}
