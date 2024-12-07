namespace AdventOfCode.Solvers.Year2024
{
    class Day7Solver : AbstractSolver
    {
        List<(long result, List<long> numbers)> Formulas = [];
        List<string> Operators = ["+", "*"];

        public override string Part1()
        {
            long result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    long[] line = input.ReadLine()!.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(n => long.Parse(n)).ToArray();

                    Formulas.Add((line[0], []));
                    for (int i = 1; i < line.Length; i++)
                    {
                        Formulas[^1].numbers.Add(line[i]);
                    }
                }
            }

            foreach ((long result, List<long> numbers) formula in Formulas)
            {
                if (GetNumberOfSolutions(formula.numbers, formula.result) > 0) result += formula.result;
            }

            return result.ToString();
        }

        private int GetNumberOfSolutions(List<long> formula, long result)
        {
            int possibleSolutions = 0;
            long testResult = formula[0];
            foreach(string op in Operators)
            {
                List<long> recFormula = new(formula);
                switch(op)
                {
                    case "+":
                        recFormula[0] += recFormula[1];
                        break;
                    case "*":
                        recFormula[0] *= recFormula[1];
                        break;
                    case "||":
                        recFormula[0] = long.Parse(recFormula[0].ToString() + recFormula[1].ToString());
                        break;
                }
                recFormula.RemoveAt(1);
                if (recFormula.Count > 1) possibleSolutions += GetNumberOfSolutions(recFormula, result);
                else if (recFormula[0] == result) possibleSolutions++;
            }

            return possibleSolutions;
        }



        public override string Part2()
        {
            long result = 0;

            Operators.Add("||");

            foreach ((long result, List<long> numbers) formula in Formulas)
            {
                if (GetNumberOfSolutions(formula.numbers, formula.result) > 0) result += formula.result;
            }

            return result.ToString();
        }
    }
}
