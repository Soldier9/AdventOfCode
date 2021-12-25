using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day24Solver : AbstractSolver
    {
        class ALU
        {
            public long AcceptedInput = 0;
            private readonly List<(string instr, string p1, string p2)> Program = new();

            public ALU(string[] program)
            {
                foreach (string line in program)
                {
                    string[] comps = line.Split(' ');
                    Program.Add((comps[0], comps[1], comps.Length > 2 ? comps[2] : "0"));
                }
            }

            private readonly Dictionary<(long W, long X, long Y, long Z, int PC), long> Cache = new();
            public long Run(Dictionary<char, long> registers, int pc, Queue<int> inputsSoFar, bool part2 = false)
            {
                if (pc == Program.Count)
                {
                    if (registers['z'] == 0)
                    {
                        StringBuilder sb = new();
                        while (inputsSoFar.Count > 0) _ = sb.Append(inputsSoFar.Dequeue());
                        AcceptedInput = long.Parse(sb.ToString());
                    }
                    return registers['z'];
                }
                if (Cache.ContainsKey((registers['w'], registers['x'], registers['y'], registers['z'], pc))) return Cache[(registers['w'], registers['x'], registers['y'], registers['z'], pc)];

                (string instr, string p1, string p2) currentLine = Program[pc];
                if (currentLine.instr == "inp")
                {
                    int[] testInputs = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    if (!part2) testInputs = testInputs.Reverse().ToArray();

                    foreach (int i in testInputs)
                    {
                        registers[currentLine.p1[0]] = i;
                        Queue<int> newInputsSoFar = new(inputsSoFar);
                        newInputsSoFar.Enqueue(i);
                        long result = Run(new Dictionary<char, long>(registers), pc + 1, newInputsSoFar);
                        _ = Cache.TryAdd((registers['w'], registers['x'], registers['y'], registers['z'], pc + 1), result);
                        if (result == 0) return 0;
                    }
                    return -1;
                }
                else
                {
                    long result = currentLine.instr switch
                    {
                        "add" => GetRegisterOrValue(currentLine.p1, registers) + GetRegisterOrValue(currentLine.p2, registers),
                        "mul" => GetRegisterOrValue(currentLine.p1, registers) * GetRegisterOrValue(currentLine.p2, registers),
                        "div" => GetRegisterOrValue(currentLine.p1, registers) / GetRegisterOrValue(currentLine.p2, registers),
                        "mod" => GetRegisterOrValue(currentLine.p1, registers) % GetRegisterOrValue(currentLine.p2, registers),
                        "eql" => GetRegisterOrValue(currentLine.p1, registers) == GetRegisterOrValue(currentLine.p2, registers) ? 1 : 0,
                        _ => throw new NotImplementedException(),
                    };
                    registers[currentLine.p1[0]] = result;

                    Queue<int> newInputsSoFar = new(inputsSoFar);
                    return Run(new Dictionary<char, long>(registers), pc + 1, newInputsSoFar);
                }
            }

            private static long GetRegisterOrValue(string register, Dictionary<char, long> registers)
            {
                if (int.TryParse(register, out int value)) return value;
                return registers[register[0]];
            }
        }

        public override string Part1()
        {
            string[] program = Array.Empty<string>();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    program = input.ReadToEnd()!.Split("\r\n");
                }
            }

            ALU alu = new(program);

            Dictionary<char, long> registers = new();
            registers.Add('w', 0);
            registers.Add('x', 0);
            registers.Add('y', 0);
            registers.Add('z', 0);

            Queue<int> inputs = new();
            _ = alu.Run(registers, 0, inputs);
            return alu.AcceptedInput.ToString();
        }

        public override string Part2()
        {
            string[] program = Array.Empty<string>();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    program = input.ReadToEnd()!.Split("\r\n");
                }
            }

            ALU alu = new(program);

            Dictionary<char, long> registers = new();
            registers.Add('w', 0);
            registers.Add('x', 0);
            registers.Add('y', 0);
            registers.Add('z', 0);

            Queue<int> inputs = new();
            _ = alu.Run(registers, 0, inputs, true);
            return alu.AcceptedInput.ToString();
        }
    }
}
