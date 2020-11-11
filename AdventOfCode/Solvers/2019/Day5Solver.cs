using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2019
{
    class Day5Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            int[] Program;
            int IP;

            public int RunProgram(int[] program)
            {
                IP = 0;
                Program = program;

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
                            Add(modes);
                            IP += 4;
                            break;
                        case 2:
                            Multiply(modes);
                            IP += 4;
                            break;
                        case 3:
                            Input();
                            IP += 2;
                            break;
                        case 4:
                            Output(modes);
                            IP += 2;
                            break;
                        case 5:
                            JumpIfTrue(modes);
                            break;
                        case 6:
                            JumpIfFalse(modes);
                            break;
                        case 7:
                            LessThan(modes);
                            IP += 4;
                            break;
                        case 8:
                            Equals(modes);
                            IP += 4;
                            break;

                        case 99:
                            return Program[0];
                    }
                }
            }

            void Add(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = param1 + param2;
            }

            void Multiply(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = param1 * param2;
            }

            void Input()
            {
                int param1 = Program[IP + 1]; // Always position mode

                Console.Write("\r\nWaiting for input: ");
                Program[param1] = int.Parse(Console.ReadLine());
            }

            void Output(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);

                Console.Write("Output from program: ");
                Console.Write("{0}\r\n", param1);
            }

            void JumpIfTrue(int[] modes)
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
            void JumpIfFalse(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);

                if (param1 == 0)
                {
                    IP = param2;
                } else
                {
                    IP += 3;
                }
            }

            void LessThan(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = (param1 < param2 ? 1 : 0);
            }

            void Equals(int[] modes)
            {
                int param1 = (modes[0] == 0 ? Program[Program[IP + 1]] : Program[IP + 1]);
                int param2 = (modes[1] == 0 ? Program[Program[IP + 2]] : Program[IP + 2]);
                int param3 = Program[IP + 3];

                Program[param3] = (param1 == param2 ? 1 : 0);
            }
        }

        public override string Part1()
        {
            IntcodeCPU cpu = new IntcodeCPU();
            int[] program;

            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            cpu.RunProgram(program);
            return "Result is the last output of the program";
        }

        public override string Part2()
        {
            IntcodeCPU cpu = new IntcodeCPU();
            int[] program;

            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            cpu.RunProgram(program);
            return "Result is the last output of the program";
        }
    }
}
