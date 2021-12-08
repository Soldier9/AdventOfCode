using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day8Solver : AbstractSolver
    {
        public override string Part1()
        {
            int result = 0;

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine().Split('|')[1];
                    foreach (string display in line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        switch (display.Length)
                        {
                            case 2: // 1
                            case 4: // 4
                            case 3: // 7
                            case 7: result++; break; // 8
                        }
                    }
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    ISet<char>[] digits = new HashSet<char>[10];
                    List<ISet<char>> unplacedDigits = new List<ISet<char>>();

                    string[] line = input.ReadLine().Split('|');
                    foreach (string display in line[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ISet<char> newDigit = new HashSet<char>(display);
                        switch (display.Length)
                        {
                            case 2: digits[1] = newDigit; break; // 1
                            case 4: digits[4] = newDigit; break; // 4
                            case 3: digits[7] = newDigit; break; // 7
                            case 7: digits[8] = newDigit; break; // 8
                            default: unplacedDigits.Add(newDigit); break;
                        }
                    }

                    digits[3] = unplacedDigits.Single(d => d.Count == 5 && digits[1].IsSubsetOf(d));
                    unplacedDigits.Remove(digits[3]);

                    digits[9] = unplacedDigits.Single(d => d.Count == 6 && digits[3].IsSubsetOf(d));
                    unplacedDigits.Remove(digits[9]);

                    digits[5] = unplacedDigits.Single(d => d.Count == 5 && d.IsSubsetOf(digits[9]));
                    unplacedDigits.Remove(digits[5]);

                    digits[6] = unplacedDigits.Single(d => d.Count == 6 && digits[5].IsSubsetOf(d));
                    unplacedDigits.Remove(digits[6]);

                    digits[0] = unplacedDigits.Single(d => d.Count == 6);
                    unplacedDigits.Remove(digits[0]);

                    digits[2] = unplacedDigits.Single(d => d.Count == 5);
                    unplacedDigits.Remove(digits[2]);

                    string value = "";
                    foreach (string display in line[1].Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
                    {
                        ISet<char> testDigit = new HashSet<char>(display);
                        for (int i = 0; i < 10; i++) if(testDigit.SetEquals(digits[i])) value += i.ToString();
                    }

                    result += int.Parse(value);
                }
            }

            return result.ToString();
        }
    }
}
