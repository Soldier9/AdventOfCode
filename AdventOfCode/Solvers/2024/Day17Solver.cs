using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2024
{
    class Day17Solver : AbstractSolver
    {
        class ElfPuter
        {
            public List<int> Program = [];
            public Dictionary<char, long> Registers = new()
            {
                { 'A', 0 },
                { 'B', 0 },
                { 'C', 0 }
            };
            int InstructionPointer = 0;

            public List<int> Outputs = [];

            static Regex InputParser = new(@"Register A: (\d+)\r?\nRegister B: (\d+)\r?\nRegister C: (\d+)(?:\r?\n){2}Program: ((?:,?\d)+)$", RegexOptions.Multiline);
            public ElfPuter(string input)
            {
                Match parsedInput = InputParser.Match(input);
                Program = parsedInput.Groups[4].Value.Split(',').Select(x => int.Parse(x)).ToList();
                Registers['A'] = int.Parse(parsedInput.Groups[1].Value);
                Registers['B'] = int.Parse(parsedInput.Groups[2].Value);
                Registers['C'] = int.Parse(parsedInput.Groups[3].Value);
            }

            public ElfPuter(ElfPuter elfPuter)
            {
                Program = new(elfPuter.Program);
                Registers = new(elfPuter.Registers);
                InstructionPointer = elfPuter.InstructionPointer;
            }

            public void Run()
            {
                while (InstructionPointer < Program.Count)
                {
                    int opCode = Program[InstructionPointer++];
                    int operand = Program[InstructionPointer++];

                    switch (opCode)
                    {
                        case 0: //adv COMBO operand
                            Registers['A'] = Registers['A'] >>> (int)ComboOperand(operand);
                            break;
                        case 1: //bxl
                            Registers['B'] = Registers['B'] ^ operand;
                            break;
                        case 2: //bst COMBO operand
                            Registers['B'] = ComboOperand(operand) & 0b111;
                            break;
                        case 3: //jnz
                            if (Registers['A'] != 0) InstructionPointer = operand;
                            break;
                        case 4: //bxc
                            Registers['B'] = Registers['B'] ^ Registers['C'];
                            break;
                        case 5: //out COMBO operand
                            Outputs.Add((int)(ComboOperand(operand) & 0b111));
                            break;
                        case 6: //bdv
                            Registers['B'] = Registers['A'] >>> (int)ComboOperand(operand);
                            break;
                        case 7: //cdv
                            Registers['C'] = Registers['A'] >>> (int)ComboOperand(operand);
                            break;
                    }
                }
            }

            public long ComboOperand(int operand)
            {
                if (operand <= 3) return operand;
                return operand switch
                {
                    4 => Registers['A'],
                    5 => Registers['B'],
                    6 => Registers['C'],
                    7 => throw new NotImplementedException(),
                    _ => throw new NotImplementedException()
                };
            }

            public string GetOutput() => String.Join(',', Outputs);
        }


        public override string Part1()
        {
            ElfPuter elfPuter;

            using (StreamReader input = File.OpenText(InputFile))
            {
                elfPuter = new(input.ReadToEnd());
            }
            elfPuter.Run();

            return elfPuter.GetOutput();
        }


        public override string Part2()
        {
            ElfPuter elfPuter;

            using (StreamReader input = File.OpenText(InputFile))
            {
                elfPuter = new(input.ReadToEnd());
            }

            List<long> candidates = [0];
            for (int i = elfPuter.Program.Count - 1; i >= 0; i--)
            {
                List<long> newCandidates = [];
                foreach (long candidate in candidates)
                {
                    for (long n = 0; n <= 0b111; n++)
                    {
                        long newCandidate = (candidate << 3) + n;
                        ElfPuter testPuter = new(elfPuter);
                        testPuter.Registers['A'] = newCandidate;
                        testPuter.Run();

                        if (testPuter.Outputs.SequenceEqual(elfPuter.Program[i..])) newCandidates.Add(newCandidate);
                    }
                }
                candidates = newCandidates;
            }

            return candidates.Min().ToString();
        }
    }
}
