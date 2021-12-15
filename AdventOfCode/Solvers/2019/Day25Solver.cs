using System.Collections;
using System.Collections.Concurrent;

namespace AdventOfCode.Solvers.Year2019
{
    class Day25Solver : AbstractSolver
    {
        class IntcodeCPU
        {
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

            public static IEnumerable<long> GetIntCodeInput(string input)
            {
                foreach (char c in input)
                {
                    yield return Convert.ToInt64(c);
                }
            }
        }


        public override string Part1()
        {
            long[] program;
            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => long.Parse(n)).ToArray();
            }

            BlockingCollection<long> inputQueue = new();
            BlockingCollection<long> outputQueue = new();

            IntcodeCPU cpu = new(program, inputQueue, outputQueue);

            Thread cpuThread = new(new ThreadStart(cpu.RunProgram));
            cpuThread.Start();

            _ = Task.Factory.StartNew(() =>
              {
                  while (cpuThread.IsAlive || outputQueue.Count > 0)
                  {
                      if (outputQueue.Count > 0) Console.Write((char)outputQueue.Take());
                  }
              });


            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take cake\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take fuel cell\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take easter egg\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take ornament\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take hologram\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take dark matter\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take klein bottle\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("take hypercube\n")) inputQueue.Add(c);
            foreach (long c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);

            List<string> items = new() { "cake", "fuel cell", "easter egg", "ornament", "hologram", "dark matter", "klein bottle", "hypercube" };

            foreach (byte perm in Perms())
            {
                while (outputQueue.Count > 0 && inputQueue.Count > 0) { }
                DropAll(items, inputQueue);
                while (outputQueue.Count > 0 && inputQueue.Count > 0) { }
                int g = 0;
                foreach (bool b in new BitArray(new byte[] { perm }))
                {
                    if (b) foreach (long n in IntcodeCPU.GetIntCodeInput("take " + items[g] + "\n")) inputQueue.Add(n);
                    g++;
                }
                foreach (long n in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(n);

            }


            while (true)
            {
                foreach (long n in IntcodeCPU.GetIntCodeInput(Console.ReadLine() + "\n"))
                {
                    inputQueue.Add(n);
                }
            }
        }

        static IEnumerable<byte> Perms()
        {
            for (int i0 = 0; i0 < 2; i0++)
            {
                for (int i1 = 0; i1 < 2; i1++)
                {
                    for (int i2 = 0; i2 < 2; i2++)
                    {
                        for (int i3 = 0; i3 < 2; i3++)
                        {
                            for (int i4 = 0; i4 < 2; i4++)
                            {
                                for (int i5 = 0; i5 < 2; i5++)
                                {
                                    for (int i6 = 0; i6 < 2; i6++)
                                    {
                                        for (int i7 = 0; i7 < 2; i7++)
                                        {
                                            yield return (byte)(i0 * 128 + i1 * 64 + i2 * 32 + i3 * 16 + i4 * 8 + i5 * 4 + i6 * 2 + i7);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void DropAll(List<string> items, BlockingCollection<long> input)
        {
            foreach (string item in items) foreach (long c in IntcodeCPU.GetIntCodeInput("drop " + item + "\n")) input.Add(c);
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
