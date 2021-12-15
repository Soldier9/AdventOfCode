namespace AdventOfCode.Solvers.Year2021
{
    class Day2Solver : AbstractSolver
    {
        public override string Part1()
        {
            int horizontal = 0;
            int deepth = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] instr = input.ReadLine()!.Split(new char[] { ' ' });
                    switch (instr[0])
                    {
                        case "forward": horizontal += int.Parse(instr[1]); break;
                        case "down": deepth += int.Parse(instr[1]); break;
                        case "up": deepth -= int.Parse(instr[1]); break;
                    }
                }
            }

            return (horizontal * deepth).ToString();
        }

        public override string Part2()
        {
            int horizontal = 0;
            int aim = 0;
            int deepth = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] instr = input.ReadLine()!.Split(new char[] { ' ' });
                    switch (instr[0])
                    {
                        case "forward":
                            horizontal += int.Parse(instr[1]);
                            deepth += (int.Parse(instr[1]) * aim);
                            break;
                        case "down": aim += int.Parse(instr[1]); break;
                        case "up": aim -= int.Parse(instr[1]); break;

                    }
                }
            }

            return (horizontal * deepth).ToString();
        }
    }
}
