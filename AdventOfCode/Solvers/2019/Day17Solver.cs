using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solvers.Year2019
{
    class Day17Solver : AbstractSolver
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
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            int width = 0;
            int height = 0;
            List<string> lines = new List<string>();

            var sb = new StringBuilder();
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                if (outputQueue.Count > 0)
                {
                    char c = (char)outputQueue.Take();
                    if (c == 10)
                    {
                        if (width == 0) width = sb.Length;
                        if (sb.Length == width) lines.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            height = lines.Count;

            int alignmentParamSum = 0;
            for (var x = 1; x < width - 1; x++)
            {
                for (var y = 1; y < height - 1; y++)
                {
                    if (lines[y][x] == '#' &&
                        lines[y - 1][x] == '#' &&
                        lines[y + 1][x] == '#' &&
                        lines[y][x - 1] == '#' &&
                        lines[y][x + 1] == '#')
                    {
                        alignmentParamSum += (x * y);
                    }
                }
            }

            return alignmentParamSum.ToString();
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
            BlockingCollection<Int64> inputQueue = new BlockingCollection<Int64>();

            IntcodeCPU cpu = new IntcodeCPU(program, inputQueue, outputQueue);
            Task<Int64> cpuTask = new Task<Int64>(() =>
            {
                return cpu.RunProgram();
            });
            cpuTask.Start();

            string mainRoutine = "B,C,B,C,A,B,C,A,B,A\n";
            string funA = "L,10,R,6,R,6,L,8\n";
            string funB = "R,6,L,10,R,8\n";
            string funC = "R,8,R,12,L,8,L,8\n";

            foreach (var n in GetIntCodeInput(mainRoutine)) inputQueue.Add(n);
            foreach (var n in GetIntCodeInput(funA)) inputQueue.Add(n);
            foreach (var n in GetIntCodeInput(funB)) inputQueue.Add(n);
            foreach (var n in GetIntCodeInput(funC)) inputQueue.Add(n);
            foreach (var n in GetIntCodeInput("n\n")) inputQueue.Add(n);

            Int64 result = 0;
            while (!cpuTask.IsCompleted || outputQueue.Count > 0)
            {
                if (outputQueue.Count == 0) continue;
                Int64 output = 0;
                output = outputQueue.Take();
                if (output < 127) Console.Write((char)output);
                else result = output;
            }

            return result.ToString();
        }

        public IEnumerable<Int64> GetIntCodeInput(string input)
        {
            foreach (var c in input)
            {
                yield return Convert.ToInt64(c);
            }
        }
    }
}
