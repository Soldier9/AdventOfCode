using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day21Solver : AbstractSolver
    {
        public override string Part1()
        {
            Dictionary<string, BigInteger> monkeys = new();
            Dictionary<string, (string, string, string)> delayedMonkeys = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parser = new(@"(\w+): ((\d+)|(\w+) (.) (\w+))");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    Match m = parser.Match(line);
                    if (m.Groups[3].Success) monkeys.Add(m.Groups[1].Value, int.Parse(m.Groups[3].Value!));
                    else delayedMonkeys.Add(m.Groups[1].Value, (m.Groups[4].Value, m.Groups[5].Value, m.Groups[6].Value));
                }
            }

            while (delayedMonkeys.Count > 0)
            {
                foreach (KeyValuePair<string, (string m1, string o, string m2)> monkey in delayedMonkeys)
                {
                    if (monkeys.ContainsKey(monkey.Value.m1) && monkeys.ContainsKey(monkey.Value.m2))
                    {
                        BigInteger result = 0;
                        switch (monkey.Value.o)
                        {
                            case "+":
                                result = monkeys[monkey.Value.m1] + monkeys[monkey.Value.m2];
                                break;
                            case "-":
                                result = monkeys[monkey.Value.m1] - monkeys[monkey.Value.m2];
                                break;
                            case "*":
                                result = monkeys[monkey.Value.m1] * monkeys[monkey.Value.m2];
                                break;
                            case "/":
                                result = monkeys[monkey.Value.m1] / monkeys[monkey.Value.m2];
                                break;
                        }
                        monkeys.Add(monkey.Key, result);
                        delayedMonkeys.Remove(monkey.Key);
                    }
                }
            }

            return monkeys["root"].ToString();
        }

        public override string Part2()
        {
            Dictionary<string, BigInteger> monkeys = new();
            Dictionary<string, (string m1, string o, string m2)> delayedMonkeys = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parser = new(@"(\w+): ((\d+)|(\w+) (.) (\w+))");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    Match m = parser.Match(line);
                    if (m.Groups[3].Success) monkeys.Add(m.Groups[1].Value, int.Parse(m.Groups[3].Value!));
                    else delayedMonkeys.Add(m.Groups[1].Value, (m.Groups[4].Value, m.Groups[5].Value, m.Groups[6].Value));
                }
            }

            Dictionary<string, BigInteger> wanted = new();
            monkeys.Remove("humn");

            while (!wanted.ContainsKey("humn"))
            {
                foreach (KeyValuePair<string, (string m1, string o, string m2)> monkey in delayedMonkeys)
                {
                    if(monkey.Key == "root")
                    {
                        if (monkeys.ContainsKey(monkey.Value.m1))
                        {
                            wanted.Add(monkey.Value.m2, monkeys[monkey.Value.m1]);
                            delayedMonkeys.Remove("root");
                        }
                        else if(monkeys.ContainsKey(monkey.Value.m2))
                        {
                            wanted.Add(monkey.Value.m1, monkeys[monkey.Value.m2]);
                            delayedMonkeys.Remove("root");
                        }
                    } 
                    
                    else if (monkeys.ContainsKey(monkey.Value.m1) && monkeys.ContainsKey(monkey.Value.m2))
                    {
                        BigInteger result = 0;
                        switch (monkey.Value.o)
                        {
                            case "+":
                                result = monkeys[monkey.Value.m1] + monkeys[monkey.Value.m2];
                                break;
                            case "-":
                                result = monkeys[monkey.Value.m1] - monkeys[monkey.Value.m2];
                                break;
                            case "*":
                                result = monkeys[monkey.Value.m1] * monkeys[monkey.Value.m2];
                                break;
                            case "/":
                                result = monkeys[monkey.Value.m1] / monkeys[monkey.Value.m2];
                                break;
                        }
                        monkeys.Add(monkey.Key, result);
                        delayedMonkeys.Remove(monkey.Key);
                    } 
                    
                    else if(wanted.ContainsKey(monkey.Key)) {
                        if (monkeys.ContainsKey(monkey.Value.m1))
                        {
                            switch (monkey.Value.o)
                            {
                                case "+":
                                    wanted.Add(monkey.Value.m2, wanted[monkey.Key] - monkeys[monkey.Value.m1]);
                                    break;
                                case "-":
                                    wanted.Add(monkey.Value.m2, monkeys[monkey.Value.m1] - wanted[monkey.Key]);
                                    break;
                                case "*":
                                    wanted.Add(monkey.Value.m2, wanted[monkey.Key] / monkeys[monkey.Value.m1]);
                                    break;
                                case "/":
                                    wanted.Add(monkey.Value.m2, monkeys[monkey.Value.m1] / wanted[monkey.Key]);
                                    break;
                            }
                            delayedMonkeys.Remove(monkey.Key);
                        } 
                        else if (monkeys.ContainsKey(monkey.Value.m2))
                        {
                            switch (monkey.Value.o)
                            {
                                case "+":
                                    wanted.Add(monkey.Value.m1, wanted[monkey.Key] - monkeys[monkey.Value.m2]);
                                    break;
                                case "-":
                                    wanted.Add(monkey.Value.m1, wanted[monkey.Key] + monkeys[monkey.Value.m2]);
                                    break;
                                case "*":
                                    wanted.Add(monkey.Value.m1, wanted[monkey.Key] / monkeys[monkey.Value.m2]);
                                    break;
                                case "/":
                                    wanted.Add(monkey.Value.m1, wanted[monkey.Key] * monkeys[monkey.Value.m2]);
                                    break;
                            }
                            delayedMonkeys.Remove(monkey.Key);
                        }
                    }
                }
            }

            return wanted["humn"].ToString();
        }
    }
}
