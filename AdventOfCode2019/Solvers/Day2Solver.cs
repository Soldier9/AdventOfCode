using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day2Solver : AbstractSolver
    {
        class IntcodeCPU
        {
            int[] Program;
            int IP = 0;

            public int RunProgram(int[] program)
            {
                Program = program;

                bool terminate = false;
                while(!terminate)
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
                            terminate = true;
                            break;
                    }
                    IP += 4;
                }

                return Program[0];
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
            int[] program;
            using (var input = File.OpenText(InputFile))
            {
                program = input.ReadLine().Split(',').Select(n => int.Parse(n)).ToArray();
            }

            bool terminate = false;
            int result = 0;
            for (int n = 0; n < 100; n++)
            {
                for (int v = 0; v < 100; v++)
                {
                    int[] testProgram = new int[program.Length];
                    program.CopyTo(testProgram, 0);

                    testProgram[1] = n;
                    testProgram[2] = v;

                    IntcodeCPU cpu = new IntcodeCPU();
                    terminate = (cpu.RunProgram(testProgram) == 19690720);
                    if (terminate)
                    {
                        result = n * 100 + v;
                        break;
                    }
                }
                if (terminate) break;
            }

            return result.ToString();

        }
    }
}
