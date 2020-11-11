using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2019
{
    class Day7Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            private class YieldException : Exception { }

            readonly int[] Program;
            int IP;
            readonly Queue<int> InputQueue = new Queue<int>();
            readonly Queue<int> OutputQueue = new Queue<int>();
            int? LastOutput;
            public bool HasTerminated = false;

            public IntcodeCPU(int[] program)
            {
                IP = 0;
                Program = (int[])program.Clone();
            }

            public void Input(int input)
            {
                InputQueue.Enqueue(input);
            }
            
            public Queue<int> ResumeProgram()
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
                            try
                            {
                                InputInstr();
                            }
                            catch (YieldException)
                            {
                                return OutputQueue;
                            }
                            IP += 2;
                            break;
                        case 4:
                            OutputQueue.Enqueue(OutputInstr(modes));
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
                            return OutputQueue;
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

                if(InputQueue.Count > 0)
                {
                    Program[param1] = InputQueue.Dequeue();
                } else
                {
                    throw new YieldException();
                }
            }

            int OutputInstr(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                LastOutput = param1;
                return param1;
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

            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            int greatestFinalResult = 0;
            foreach (int[] settings in GetCombinations())
            {
                Queue<int> outputs = new Queue<int>();
                outputs.Enqueue(0); // First input for CPU A
                
                for (int cpuNum = 0; cpuNum < 5; cpuNum++)
                {
                    IntcodeCPU cpu = new IntcodeCPU(program);
                    cpu.Input(settings[cpuNum]);
                    cpu.Input(outputs.Dequeue());

                    outputs = cpu.ResumeProgram();
                }

                while (outputs.Count > 1) outputs.Dequeue(); // We only want the final output!
                if (outputs.Peek() > greatestFinalResult) greatestFinalResult = outputs.Dequeue();
            }

            return greatestFinalResult.ToString();
        }

        public override string Part2()
        {
            int[] program;

            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            int greatestFinalResult = 0;
            foreach (int[] settings in GetCombinations2())
            {
                IntcodeCPU[] cpus = new IntcodeCPU[5];
                for (int i = 0; i < 5; i++)
                {
                    cpus[i] = new IntcodeCPU(program);
                    cpus[i].Input(settings[i]);
                }

                cpus[0].Input(0);
                Queue<int> outputs = new Queue<int>();
                
                while (!cpus[4].HasTerminated) { 
                    for (int i = 0; i < 5; i++)
                    {
                        while (outputs.Count > 0) cpus[i].Input(outputs.Dequeue());
                        outputs = cpus[i].ResumeProgram();
                    }
                }

                while (outputs.Count > 1) outputs.Dequeue(); // We only want the final output!
                if (outputs.Peek() > greatestFinalResult) greatestFinalResult = outputs.Dequeue();
            }

            return greatestFinalResult.ToString();
        }
    }
}
