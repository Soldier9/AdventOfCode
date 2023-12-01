using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solvers.Year2023
{
    class Day1Solver : AbstractSolver
    {

        public override string Part1()
        {
            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;

                    string firstDigit = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        firstDigit = line[i].ToString();
                        if (int.TryParse(firstDigit, out int test)) break;
                    }

                    string lastDigit = "";
                    for (int i = line.Length - 1; i >= 0; i--)
                    {
                        lastDigit = line[i].ToString();
                        if (int.TryParse(lastDigit, out int test)) break;
                    }

                    result += int.Parse(firstDigit + lastDigit);
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            List<string> words = new()
            {
                "zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine"
            };

            int result = 0;
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;

                    string firstDigit = "";
                    int firstDigitPos = -1;
                    for (firstDigitPos = 0; firstDigitPos < line.Length; firstDigitPos++)
                    {
                        firstDigit = line[firstDigitPos].ToString();
                        if (int.TryParse(firstDigit, out int test)) break;
                    }
                    foreach (string word in words)
                    {
                        if (line.IndexOf(word) > -1 && line.IndexOf(word) < firstDigitPos)
                        {
                            firstDigitPos = line.IndexOf(word);
                            firstDigit = words.IndexOf(word).ToString();
                        }
                    }

                    string lastDigit = "";
                    int lastDigitPos = -1;
                    for (lastDigitPos = line.Length - 1; lastDigitPos >= 0; lastDigitPos--)
                    {
                        lastDigit = line[lastDigitPos].ToString();
                        if (int.TryParse(lastDigit, out int test)) break;
                    }
                    foreach (string word in words)
                    {
                        if (line.LastIndexOf(word) > -1 && line.LastIndexOf(word) > lastDigitPos)
                        {
                            lastDigitPos = line.LastIndexOf(word);
                            lastDigit = words.IndexOf(word).ToString();
                        }
                    }

                    result += int.Parse(firstDigit + lastDigit);
                }
            }

            return result.ToString();
        }
    }
}
