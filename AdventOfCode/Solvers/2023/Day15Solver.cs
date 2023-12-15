namespace AdventOfCode.Solvers.Year2023
{
    class Day15Solver : AbstractSolver
    {
        List<string> instructions = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    instructions.AddRange(line.Split(','));
                }
            }

            long result = 0;
            foreach (string instruction in instructions)
            {
                result += Hash(instruction);
            }

            return result.ToString();
        }

        private static int Hash(string input)
        {
            int result = 0;
            foreach (char c in input)
            {
                result += (int)c;
                result *= 17;
                result %= 256;
            }
            return result;
        }

        public override string Part2()
        {
            Dictionary<int, Dictionary<string, (int position, int focalLength)>> boxes = new();
            foreach (string instruction in instructions)
            {
                string label = instruction.Split("-=".ToCharArray())[0];
                int boxNum = Hash(label);
                if (!boxes.ContainsKey(boxNum)) boxes.Add(boxNum, new());
                Dictionary<string, (int position, int focalLength)> box = boxes[boxNum];

                if (instruction.Contains('-') && box.ContainsKey(label))
                {
                    foreach (string lensLabel in box.Where(l => l.Value.position > box[label].position).Select(l => l.Key))
                    {
                        box[lensLabel] = (box[lensLabel].position - 1, box[lensLabel].focalLength);
                    }
                    box.Remove(label);
                }
                else if (instruction.Contains('='))
                {
                    int focalLength = int.Parse(instruction.Split('=')[1]);
                    if (box.ContainsKey(label)) box[label] = (box[label].position, focalLength);
                    else box.Add(label, (box.Count + 1, focalLength));
                }
            }

            int result = 0;
            foreach (KeyValuePair<int, Dictionary<string, (int position, int focalLength)>> box in boxes)
            {
                foreach (var lens in box.Value.Values)
                {
                    int focus = 1 + box.Key;
                    focus *= lens.position;
                    focus *= lens.focalLength;
                    result += focus;
                }
            }

            return result.ToString();
        }
    }
}