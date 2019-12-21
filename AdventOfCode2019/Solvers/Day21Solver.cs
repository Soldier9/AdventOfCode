using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day21Solver : AbstractSolver
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

            public static IEnumerable<Int64> GetIntCodeInput(string input)
            {
                foreach (var c in input)
                {
                    yield return Convert.ToInt64(c);
                }
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
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            List<string> springScript = new List<string>();
            springScript.Add("NOT C J\n");
            springScript.Add("NOT B T\n");
            springScript.Add("OR T J\n");
            springScript.Add("NOT A T\n");
            springScript.Add("OR T J\n");
            springScript.Add("AND D J\n");
            springScript.Add("WALK\n");
            foreach(var line in springScript) foreach (var c in IntcodeCPU.GetIntCodeInput(line)) inputQueue.Add(c);

            Int64 result = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                while (outputQueue.Count > 0)
                {
                    var output = outputQueue.Take();
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
            Int64[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => Int64.Parse(n)).ToArray();
            }
            BlockingCollection<Int64> outputQueue = new BlockingCollection<Int64>();
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            List<string> springScript = new List<string>();
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
            foreach (var line in springScript) foreach (var c in IntcodeCPU.GetIntCodeInput(line)) inputQueue.Add(c);

            Int64 result = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                while (outputQueue.Count > 0)
                {
                    var output = outputQueue.Take();
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
