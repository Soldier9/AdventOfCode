namespace AdventOfCode.Solvers.Year2022
{
    class Day2Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] actions = input.ReadLine()!.Split(" ");
                    switch (actions[1])
                    {
                        case "X":
                            result += 1;
                            switch (actions[0])
                            {
                                case "A": result += 3; break;
                                case "B": break;
                                case "C": result += 6; break;
                            }
                            break;
                        case "Y":
                            result += 2;
                            switch (actions[0])
                            {
                                case "A": result += 6; break;
                                case "B": result += 3; break;
                                case "C": break;
                            }
                            break;
                        case "Z":
                            result += 3;
                            switch (actions[0])
                            {
                                case "A": break;
                                case "B": result += 6; break;
                                case "C": result += 3; break;
                            }
                            break;
                    }
                }
            }
            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] actions = input.ReadLine()!.Split(" ");
                    switch (actions[1])
                    {
                        case "X":
                            switch (actions[0])
                            {
                                case "A": result += 3; break;
                                case "B": result += 1; break;
                                case "C": result += 2; break;
                            }
                            break;
                        case "Y":
                            result += 3;
                            switch (actions[0])
                            {
                                case "A": result += 1; break;
                                case "B": result += 2; break;
                                case "C": result += 3; break;
                            }
                            break;
                        case "Z":
                            result += 6;
                            switch (actions[0])
                            {
                                case "A": result += 2; break;
                                case "B": result += 3; break;
                                case "C": result += 1; break;
                            }
                            break;
                    }
                }
            }
            return result.ToString();
        }
    }
}
