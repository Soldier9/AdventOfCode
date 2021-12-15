using System.Collections.Concurrent;

namespace AdventOfCode.Solvers.Year2019
{
    class Day7SolverThreaded : AbstractSolver
    {
        public override bool PrioritizedSolver => true;

        class IntcodeCPU
        {
            readonly int[] Program;
            int IP;
            readonly BlockingCollection<int> InputQueue;
            readonly BlockingCollection<int> OutputQueue;
            int? LastOutput;
            public bool HasTerminated = false;

            public IntcodeCPU(int[] program, BlockingCollection<int> input, BlockingCollection<int> output)
            {
                IP = 0;
                Program = (int[])program.Clone();
                InputQueue = input;
                OutputQueue = output;
            }

            public void Input(int input)
            {
                InputQueue.Add(input);
            }

            public int RunProgram()
            {
                while (true)
                {
                    int opCode = Program[IP];

                    int[] modes = new int[3];
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
                            InputInstr();
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

                        case 99:
                            HasTerminated = true;
                            return (LastOutput != null ? (int)LastOutput : -1);
                    }
                }
            }

            void AddInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = param1 + param2;
            }

            void MultInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = param1 * param2;
            }

            void InputInstr()
            {
                int param1 = Program[IP + 1]; // Always position mode
                Program[param1] = InputQueue.Take();
            }

            void OutputInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                LastOutput = param1;
                OutputQueue.Add(param1);
            }

            void JumpIfTrueInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);

                if (param1 != 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }
            void JumpIfFalseInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);

                if (param1 == 0)
                {
                    IP = param2;
                }
                else
                {
                    IP += 3;
                }
            }

            void LessThanInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = (param1 < param2 ? 1 : 0);
            }

            void EqualsInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = (param1 == param2 ? 1 : 0);
            }

            public override string ToString()
            {
                return "IP: " + IP.ToString() + ", LastOutput: " + LastOutput + ", Halted: " + HasTerminated;
            }
        }

        public static IEnumerable<int[]> GetCombinations()
        {
            int[] phaseSettings = new int[5];
            for (int i0 = 0; i0 < 5; i0++)
            {
                phaseSettings[0] = i0;
                for (int i1 = 0; i1 < 5; i1++)
                {
                    if (i0 == i1) continue;
                    phaseSettings[1] = i1;
                    for (int i2 = 0; i2 < 5; i2++)
                    {
                        if (i0 == i2 || i1 == i2) continue;
                        phaseSettings[2] = i2;
                        for (int i3 = 0; i3 < 5; i3++)
                        {
                            if (i0 == i3 || i1 == i3 || i2 == i3) continue;
                            phaseSettings[3] = i3;
                            for (int i4 = 0; i4 < 5; i4++)
                            {
                                if (i0 == i4 || i1 == i4 || i2 == i4 || i3 == i4) continue;
                                phaseSettings[4] = i4;

                                yield return phaseSettings;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<int[]> GetCombinations2()
        {
            int[] phaseSettings = new int[5];
            for (int i0 = 5; i0 < 10; i0++)
            {
                phaseSettings[0] = i0;
                for (int i1 = 5; i1 < 10; i1++)
                {
                    if (i0 == i1) continue;
                    phaseSettings[1] = i1;
                    for (int i2 = 5; i2 < 10; i2++)
                    {
                        if (i0 == i2 || i1 == i2) continue;
                        phaseSettings[2] = i2;
                        for (int i3 = 5; i3 < 10; i3++)
                        {
                            if (i0 == i3 || i1 == i3 || i2 == i3) continue;
                            phaseSettings[3] = i3;
                            for (int i4 = 5; i4 < 10; i4++)
                            {
                                if (i0 == i4 || i1 == i4 || i2 == i4 || i3 == i4) continue;
                                phaseSettings[4] = i4;

                                yield return phaseSettings;
                            }
                        }
                    }
                }
            }
        }

        public override string Part1()
        {
            int[] program;

            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => int.Parse(n)).ToArray();
            }

            int greatestFinalResult = 0;
            foreach (int[] settings in GetCombinations())
            {
                BlockingCollection<int> outputQueue;
                BlockingCollection<int> inputQueue = new();

                List<Task<int>> cpus = new();
                for (int i = 0; i < 5; i++)
                {
                    outputQueue = new BlockingCollection<int>();
                    inputQueue.Add(settings[i]);
                    if (i == 0) inputQueue.Add(0); // first input for cpu A
                    IntcodeCPU cpu = new(program, inputQueue, outputQueue);
                    cpus.Add(new Task<int>(() =>
                    {
                        return cpu.RunProgram();
                    }));
                    inputQueue = outputQueue;
                }
                for (int i = 0; i < 5; i++) cpus[i].Start();

                greatestFinalResult = Math.Max(greatestFinalResult, cpus[4].Result);
            }

            return greatestFinalResult.ToString();
        }

        public override string Part2()
        {
            int[] program;

            using (StreamReader input = File.OpenText(InputFile))
            {
                program = input.ReadLine()!.Split(',').Select(n => int.Parse(n)).ToArray();
            }

            int greatestFinalResult = 0;
            foreach (int[] settings in GetCombinations2())
            {
                BlockingCollection<int> outputQueue;
                BlockingCollection<int> inputQueue = new();

                BlockingCollection<int> cpu1Input = inputQueue;
                List<Task<int>> cpus = new();
                for (int i = 0; i < 5; i++)
                {
                    outputQueue = (i == 4 ? cpu1Input : new BlockingCollection<int>());
                    inputQueue.Add(settings[i]);
                    if (i == 0) inputQueue.Add(0); // first input for cpu A
                    IntcodeCPU cpu = new(program, inputQueue, outputQueue);
                    cpus.Add(new Task<int>(() =>
                    {
                        return cpu.RunProgram();
                    }));
                    inputQueue = outputQueue;
                }
                for (int i = 0; i < 5; i++) cpus[i].Start();

                greatestFinalResult = Math.Max(greatestFinalResult, cpus[4].Result);
            }

            return greatestFinalResult.ToString();
        }
    }
}
