using System.Text;

namespace AdventOfCode.Solvers.Year2022
{
    class Day10Solver : AbstractSolver
    {
        List<string> program = new();

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile)) while (!input.EndOfStream) program.Add(input.ReadLine()!);

            int regX = 1;
            int cycle = 1;
            int endCycle = 1;
            int strength = 0;

            string[] instruction = { "", "" };
            for (int i = 0; i < program.Count || instruction[0] != "";)
            {
                if (instruction[0] == "")
                {
                    instruction = program[i].Split(" ");
                    switch (instruction[0])
                    {
                        case "noop": endCycle = cycle; break;
                        case "addx": endCycle = cycle + 1; break;
                    }
                    i++;
                }

                switch (cycle)
                {
                    case 20:
                    case 60:
                    case 100:
                    case 140:
                    case 180:
                    case 220:
                        strength += cycle * regX; break;
                }

                if (cycle == endCycle)
                {
                    switch (instruction[0])
                    {
                        case "addx": regX += int.Parse(instruction[1]); break;
                    }
                    instruction[0] = "";
                }
                cycle++;
            }

            return strength.ToString();
        }

        public override string Part2()
        {
            int regX = 1;
            int cycle = 1;
            int endCycle = 1;
            StringBuilder sb = new();

            string[] instruction = { "", "" };
            for (int i = 0; i < program.Count || instruction[0] != "";)
            {
                if (instruction[0] == "")
                {
                    instruction = program[i].Split(" ");
                    switch (instruction[0])
                    {
                        case "noop": endCycle = cycle; break;
                        case "addx": endCycle = cycle + 1; break;
                    }
                    i++;
                }

                if (Math.Abs((cycle - 1) % 40 - regX) < 2) _ = sb.Append('#');
                else _ = sb.Append(' ');
                if (cycle % 40 == 0)
                {
                    sb.Append("\r\n");
                }

                if (cycle == endCycle)
                {
                    switch (instruction[0])
                    {
                        case "addx": regX += int.Parse(instruction[1]); break;
                    }
                    instruction[0] = "";
                }
                cycle++;
            }

            Program.PrintData(sb.ToString(), 0, true, false);

            return "Printed";
        }
    }
}
