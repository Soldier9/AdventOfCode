using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day8Solver : AbstractSolver
    {
        struct Instruction
        {
            public string opcode;
            public int arg;
        }
        class CPU
        {
            List<Instruction> memory;

            private int programCounter;
            private int accumulator;

            public CPU(List<Instruction> program)
            {
                memory = program;

                programCounter = 0;
                accumulator = 0;
            }

            public int Start(bool returnAccumulatorOnInfiniteLoop = false)
            {
                HashSet<int> executedLocations = new HashSet<int>();
                while (true)
                {
                    if (programCounter == memory.Count) return accumulator;
                    if (executedLocations.Contains(programCounter)) break;
                    executedLocations.Add(programCounter);

                    switch (memory[programCounter].opcode)
                    {
                        case "acc":
                            accumulator += memory[programCounter].arg;
                            break;
                        case "jmp":
                            programCounter += memory[programCounter].arg;
                            continue;
                        case "nop":
                            break;
                    }
                    programCounter++;
                }

                if (returnAccumulatorOnInfiniteLoop) return accumulator;
                return -1;
            }
        }

        List<Instruction> program = new List<Instruction>();
        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine().Split(' ');
                    program.Add(new Instruction { opcode = line[0], arg = int.Parse(line[1]) });
                }
            }

            CPU cpu = new CPU(program);
            return cpu.Start(true).ToString();
        }

        public override string Part2()
        {
            List<int> nops = new List<int>();
            List<int> jmps = new List<int>();

            for (int i = 0; i < program.Count; i++)
            {
                if (program[i].opcode == "nop") nops.Add(i);
                else if (program[i].opcode == "jmp") jmps.Add(i);
            }

            foreach (int location in nops)
            {
                List<Instruction> modifiedProgram = new List<Instruction>(program);
                modifiedProgram[location] = new Instruction { opcode = "jmp", arg = program[location].arg };

                CPU cpu = new CPU(modifiedProgram);
                int result = cpu.Start();
                if (result != -1) return result.ToString();
            }

            foreach (int location in jmps)
            {
                List<Instruction> modifiedProgram = new List<Instruction>(program);
                modifiedProgram[location] = new Instruction { opcode = "nop", arg = program[location].arg };

                CPU cpu = new CPU(modifiedProgram);
                int result = cpu.Start();
                if (result != -1) return result.ToString();
            }

            return "No solution found";
        }
    }
}