using System.IO;
using System.Linq;

namespace AdventOfCode2019.Solvers
{
    class Day02Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            int[] Program;
            int IP;
            public int RunProgram(int[] program)
            {
                IP = 0;
                Program = program;

                while(true)
                {
                    var opCode = Program[IP];
                    switch (opCode)
                    {
                        case 1:
                            Program[Program[IP + 3]] = Program[Program[IP + 1]] + Program[Program[IP + 2]];
                            break;
                        case 2:
                            Program[Program[IP + 3]] = Program[Program[IP + 1]] * Program[Program[IP + 2]];
                            break;
                        case 99:
                            return Program[0];
                    }
                    IP += 4;
                }
            }
        }



        public override string Part1()
        {
            IntcodeCPU cpu = new IntcodeCPU();
            int[] program;
            
            using(var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            program[1] = 1;
            program[2] = 2;
            return cpu.RunProgram(program).ToString();
        }

        public override string Part2()
        {
            IntcodeCPU cpu = new IntcodeCPU();
            int[] program;

            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            for (int n = 0; n < 100; n++)
            {
                for (int v = 0; v < 100; v++)
                {
                    int[] testProgram = new int[program.Length];
                    program.CopyTo(testProgram, 0);

                    testProgram[1] = n;
                    testProgram[2] = v;

                    if (cpu.RunProgram(testProgram) == 19690720) return (n * 100 + v).ToString();
                }
            }

            return "No solution found!";
        }
    }
}
