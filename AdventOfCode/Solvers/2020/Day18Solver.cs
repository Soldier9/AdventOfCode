using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2020
{
    class Day18Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<long> results = new List<long>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    results.Add(long.Parse(Evaluate(input.ReadLine())));
                }
            }
            return results.Sum().ToString();
        }

        private string Evaluate(string expr)
        {
            // Handle parantheses
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '(')
                {
                    for (int j = i + 1; j < expr.Length; j++)
                    {
                        if (expr[j] == '(') i = j;
                        else if (expr[j] == ')')
                        {
                            string preExpr = expr.Substring(0, i);
                            string innerExpr = expr.Substring(i + 1, j - i - 1);
                            string postExpr = expr.Substring(j + 1);

                            return Evaluate(preExpr + Evaluate(innerExpr) + postExpr);
                        }
                    }
                }
            }

            // Handle operations
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '+' || expr[i] == '*')
                {
                    int j;
                    for (j = i + 2; j < expr.Length; j++)
                    {
                        if (expr[j] == ' ') break;
                    }

                    string operand1 = expr.Substring(0, i);
                    string operand2 = expr.Substring(i + 2, j - i - 2);
                    string postExpr = expr.Substring(j);

                    if (expr[i] == '+') return Evaluate((long.Parse(operand1) + long.Parse(operand2)).ToString() + postExpr);
                    if (expr[i] == '*') return Evaluate((long.Parse(operand1) * long.Parse(operand2)).ToString() + postExpr);
                }
            }

            // Nothing to handle
            return expr;
        }


        public override string Part2()
        {
            List<long> results = new List<long>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    results.Add(long.Parse(Evaluate2(input.ReadLine())));
                }
            }
            return results.Sum().ToString();
        }

        private string Evaluate2(string expr)
        {
            // Handle parantheses
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '(')
                {
                    for (int j = i + 1; j < expr.Length; j++)
                    {
                        if (expr[j] == '(') i = j;
                        else if (expr[j] == ')')
                        {
                            string preExpr = expr.Substring(0, i);
                            string innerExpr = expr.Substring(i + 1, j - i - 1);
                            string postExpr = expr.Substring(j + 1);

                            return Evaluate2(preExpr + Evaluate2(innerExpr) + postExpr);
                        }
                    }
                }
            }

            // Handle sums
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '+')
                {
                    int j;
                    for (j = i + 2; j < expr.Length; j++)
                    {
                        if (expr[j] == ' ') break;
                    }

                    int k;
                    for (k = i - 2; k > 0; k--)
                    {
                        if (expr[k] == ' ') break;
                    }
                    if (k > 0) k++;

                    string preExpr = expr.Substring(0, k);
                    string operand1 = expr.Substring(k, i - k);
                    string operand2 = expr.Substring(i + 2, j - i - 2);
                    string postExpr = expr.Substring(j);

                    return Evaluate2(preExpr + (long.Parse(operand1) + long.Parse(operand2)).ToString() + postExpr);
                }
            }

            // Handle multiplication
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '*')
                {

                    int j;
                    for (j = i + 2; j < expr.Length; j++)
                    {
                        if (expr[j] == ' ') break;
                    }

                    string operand1 = expr.Substring(0, i);
                    string operand2 = expr.Substring(i + 2, j - i - 2);
                    string postExpr = expr.Substring(j);

                    return Evaluate2((long.Parse(operand1) * long.Parse(operand2)).ToString() + postExpr);
                }
            }

            // Nothing to handle
            return expr;
        }
    }
}
