using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day14Solver : AbstractSolver
    {
        class Polymer
        {
            public string Value;
            readonly Dictionary<string, char> Rules = new();

            public Polymer(string value)
            {
                Value = value;
            }

            public void AddRule(string rule)
            {
                Rules.Add(rule[..2], rule.Substring(6, 1)[0]);
            }

            public void Evolve()
            {
                StringBuilder newValue = new(Value[0].ToString());
                for (int i = 0; i < Value.Length - 1; i++)
                {
                    if (Rules.ContainsKey(Value.Substring(i, 2)))
                    {

                        _ = newValue.Append(Rules[Value.Substring(i, 2)]);
                        _ = newValue.Append(Value[i + 1]);
                    }
                    else
                    {
                        _ = newValue.Append(Value[i + 1]);
                    }
                }
                Value = newValue.ToString();
            }
        }

        public override string Part1()
        {
            Polymer? polymer = null;
            using (StreamReader input = File.OpenText(InputFile))
            {
                polymer = new Polymer(input.ReadLine()!);
                _ = input.ReadLine();
                while (!input.EndOfStream)
                {
                    polymer.AddRule(input.ReadLine()!);
                }
            }

            for (int i = 0; i < 10; i++) polymer.Evolve();

            Dictionary<char, int> chars = new();
            foreach (char c in polymer.Value)
            {
                if (!chars.ContainsKey(c)) chars.Add(c, 0);
                chars[c]++;
            }

            int max = 0;
            int min = int.MaxValue;
            foreach (KeyValuePair<char, int> n in chars)
            {
                if (n.Value > max) max = n.Value;
                if (n.Value < min) min = n.Value;
            }
            return (max - min).ToString();
        }


        class Rule
        {
            public static HashSet<Rule> Rules = new();

            public readonly string Name;
            public readonly string Result;
            public List<Rule> SubsequentRules = new();

            public Rule(string rule)
            {
                Name = rule[..2];
                Result = new string(new char[] { rule[0], rule[6], rule[1] });
                _ = Rules.Add(this);
            }

            public static void MatchSubsequentRules()
            {
                foreach (Rule rule in Rules)
                {
                    rule.SubsequentRules.Add(Rules.Single(r => r.Name == rule.Result[..2]));
                    rule.SubsequentRules.Add(Rules.Single(r => r.Name == rule.Result.Substring(1, 2)));
                }
            }
        }

        public override string Part2()
        {
            string initialPolymer = "";
            using (StreamReader input = File.OpenText(InputFile))
            {
                initialPolymer = input.ReadLine()!;
                _ = input.ReadLine();
                while (!input.EndOfStream) _ = new Rule(input.ReadLine()!);
            }

            Rule.MatchSubsequentRules();

            Dictionary<Rule, long> ruleCount = new();
            // First iteration
            for (int i = 0; i < initialPolymer.Length - 1; i++)
            {
                Rule matchingRule = Rule.Rules.Single(r => r.Name == initialPolymer.Substring(i, 2));
                if (!ruleCount.ContainsKey(matchingRule)) ruleCount.Add(matchingRule, 0);
                ruleCount[matchingRule]++;
            }

            // Further iterations
            for (int iteration = 2; iteration <= 40; iteration++)
            {
                Dictionary<Rule, long> newRuleCount = new();
                foreach (KeyValuePair<Rule, long> rule in ruleCount.Where(r => r.Value > 0))
                {
                    foreach (Rule nextRule in rule.Key.SubsequentRules)
                    {
                        if (!newRuleCount.ContainsKey(nextRule)) newRuleCount.Add(nextRule, 0);
                        newRuleCount[nextRule] += rule.Value;
                    }
                }
                ruleCount = newRuleCount;
            }

            Dictionary<char, long> chars = new();
            chars.Add(initialPolymer[0], 1);
            foreach (KeyValuePair<Rule, long> rule in ruleCount)
            {
                for (int i = 1; i < rule.Key.Result.Length; i++)
                {
                    if (!chars.ContainsKey(rule.Key.Result[i])) chars.Add(rule.Key.Result[i], 0);
                    chars[rule.Key.Result[i]] += rule.Value;
                }
            }

            long max = 0;
            long min = long.MaxValue;
            foreach (KeyValuePair<char, long> n in chars)
            {
                if (n.Value > max) max = n.Value;
                if (n.Value < min) min = n.Value;
            }
            return (max - min).ToString();
        }
    }
}
