namespace AdventOfCode.Solvers.Year2021
{
    class Day3Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int>? bits = null;
            int inputs = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (bits == null) bits = new List<int>(new int[line.Length]);
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '1') bits[i]++;
                    }
                    inputs++;
                }
            }

            int gamma = 0;
            int epsilon = 0;
            int half = inputs / 2;
            for (int i = 0; i < bits!.Count; i++)
            {
                if (bits[i] > half) gamma ^= (1 << (bits.Count - 1 - i));
                else epsilon ^= (1 << (bits.Count - 1 - i));
            }
            return (gamma * epsilon).ToString();
        }

        public override string Part2()
        {
            List<string> orgInputs = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream) orgInputs.Add(input.ReadLine()!);
            }

            int oxygen = 0;
            List<string> inputs = orgInputs.ToList<string>();
            for (int i = 0; i < inputs[0].Length; i++)
            {
                if (inputs.Count == 1)
                {
                    oxygen = Convert.ToInt32(inputs[0], 2);
                    break;
                }

                int ones = inputs.Count(input => input[i] == '1');
                if (ones >= inputs.Count / 2) _ = inputs.RemoveAll(s => s[i] == '0');
                else _ = inputs.RemoveAll(s => s[i] == '1');
            }


            int co2 = 0;
            inputs = orgInputs.ToList<string>();
            for (int i = 0; i < inputs[0].Length; i++)
            {
                if (inputs.Count == 1)
                {
                    co2 = Convert.ToInt32(inputs[0], 2);
                    break;
                }

                int ones = inputs.Count(input => input[i] == '1');
                if (ones < inputs.Count / 2) _ = inputs.RemoveAll(s => s[i] == '0');
                else _ = inputs.RemoveAll(s => s[i] == '1');
            }

            return (oxygen * co2).ToString();
        }
    }
}
