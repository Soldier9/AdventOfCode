using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Solvers.Year2019
{
    class Day25Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            readonly Dictionary<Int64, Int64> Program;
            Int64 IP;
            Int64 RelativeBase;

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
                Program[param1] = InputQueue.Take();
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

            var inputQueue = new BlockingCollection<Int64>();
            var outputQueue = new BlockingCollection<Int64>();

            var cpu = new IntcodeCPU(program, inputQueue, outputQueue);

            var cpuThread = new Thread(new ThreadStart(cpu.RunProgram));
            cpuThread.Start();

            Task.Factory.StartNew(() =>
            {
                while (cpuThread.IsAlive || outputQueue.Count > 0)
                {
                    if (outputQueue.Count > 0) Console.Write((char)outputQueue.Take());
                }
            });


            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take cake\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("south\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take fuel cell\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take easter egg\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take ornament\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take hologram\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take dark matter\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("east\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take klein bottle\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("take hypercube\n")) inputQueue.Add(c);
            foreach (var c in IntcodeCPU.GetIntCodeInput("north\n")) inputQueue.Add(c);

            List<string> items = new List<string> { "cake", "fuel cell", "easter egg", "ornament", "hologram", "dark matter", "klein bottle", "hypercube" };

            foreach (var perm in Perms())
            {
                while (outputQueue.Count > 0 && inputQueue.Count > 0) { }
                DropAll(items, inputQueue);
                while (outputQueue.Count > 0 && inputQueue.Count > 0) { }
                int g = 0;
                foreach (var b in new BitArray(new byte[] { perm }))
                {
                    if ((bool)b) foreach (var n in IntcodeCPU.GetIntCodeInput("take " + items[g] + "\n")) inputQueue.Add(n);
                    g++;
                }
                foreach (var n in IntcodeCPU.GetIntCodeInput("west\n")) inputQueue.Add(n);

            }


            while (true)
            {
                foreach (var n in IntcodeCPU.GetIntCodeInput(Console.ReadLine() + "\n"))
                {
                    inputQueue.Add(n);
                }
            }
        }

        IEnumerable<byte> Perms()
        {
            for (var i0 = 0; i0 < 2; i0++)
            {
                for (var i1 = 0; i1 < 2; i1++)
                {
                    for (var i2 = 0; i2 < 2; i2++)
                    {
                        for (var i3 = 0; i3 < 2; i3++)
                        {
                            for (var i4 = 0; i4 < 2; i4++)
                            {
                                for (var i5 = 0; i5 < 2; i5++)
                                {
                                    for (var i6 = 0; i6 < 2; i6++)
                                    {
                                        for (var i7 = 0; i7 < 2; i7++)
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

        void DropAll(List<string> items, BlockingCollection<Int64> input)
        {
            foreach (var item in items) foreach (var c in IntcodeCPU.GetIntCodeInput("drop " + item + "\n")) input.Add(c);
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
