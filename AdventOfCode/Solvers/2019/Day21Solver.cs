using System.Collections.Concurrent;

namespace AdventOfCode.Solvers.Year2019
{
    class Day21Solver : AbstractSolver
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
            BlockingCollection<long> outputQueue = new();
            BlockingCollection<long> inputQueue = new();

            IntcodeCPU cpu = new(program, inputQueue, outputQueue);
            Task<long> cpuTask = new(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            List<string> springScript = new();
            springScript.Add("NOT C J\n");
            springScript.Add("NOT B T\n");
            springScript.Add("OR T J\n");
            springScript.Add("NOT A T\n");
            springScript.Add("OR T J\n");
            springScript.Add("AND D J\n");
            springScript.Add("WALK\n");
            foreach (string line in springScript) foreach (long c in IntcodeCPU.GetIntCodeInput(line)) inputQueue.Add(c);

            long result = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                while (outputQueue.Count > 0)
                {
                    long output = outputQueue.Take();
                    if (output > 127)
                    {
                        result = output;
                        continue;
                    }
                    if (output == -2) continue;
                    Console.Write((char)output);
                }
            }

            return result.ToString();
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

            IntcodeCPU cpu = new(program, inputQueue, outputQueue);
            Task<long> cpuTask = new(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            List<string> springScript = new();
            springScript.Add("NOT C J\n");
            springScript.Add("NOT B T\n");
            springScript.Add("OR T J\n");
            springScript.Add("NOT A T\n");
            springScript.Add("OR T J\n");
            springScript.Add("AND D J\n");

            springScript.Add("NOT H T\n");
            springScript.Add("NOT T T\n");
            springScript.Add("OR E T\n");
            springScript.Add("AND T J\n");

            springScript.Add("RUN\n");
            foreach (string line in springScript) foreach (long c in IntcodeCPU.GetIntCodeInput(line)) inputQueue.Add(c);

            long result = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                while (outputQueue.Count > 0)
                {
                    long output = outputQueue.Take();
                    if (output > 127)
                    {
                        result = output;
                        continue;
                    }
                    if (output == -2) continue;
                    Console.Write((char)output);
                }
            }

            return result.ToString();
        }
    }
}
