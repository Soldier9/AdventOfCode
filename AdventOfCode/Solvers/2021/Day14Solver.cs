using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day14Solver : AbstractSolver
    {
        class Polymer
        {
            public string Value;
            Dictionary<string, char> Rules = new Dictionary<string, char>();

            public Polymer(string value)
            {
                Value = value;
            }

            public void AddRule(string rule)
            {
                Rules.Add(rule.Substring(0, 2), rule.Substring(6, 1)[0]);
            }

            public void Evolve()
            {
                StringBuilder newValue = new StringBuilder(Value[0].ToString());
                for (int i = 0; i < Value.Length - 1; i++)
                {
                    if (Rules.ContainsKey(Value.Substring(i, 2)))
                    {

                        newValue.Append(Rules[Value.Substring(i, 2)]);
                        newValue.Append(Value[i + 1]);
                    }
                    else
                    {
                        newValue.Append(Value[i + 1]);
                    }
                }
                Value = newValue.ToString();
            }
        }

        public override string Part1()
        {
            Polymer polymer = null;
            using (var input = File.OpenText(InputFile))
            {
                polymer = new Polymer(input.ReadLine());
                input.ReadLine();
                while (!input.EndOfStream)
                {
                    polymer.AddRule(input.ReadLine());
                }
            }

            for (int i = 0; i < 10; i++) polymer.Evolve();

            Dictionary<char, int> chars = new Dictionary<char, int>();
            foreach (char c in polymer.Value)
            {
                if (!chars.ContainsKey(c)) chars.Add(c, 0);
                chars[c]++;
            }

            int max = 0;
            int min = int.MaxValue;
            foreach (var n in chars)
            {
                if (n.Value > max) max = n.Value;
                if (n.Value < min) min = n.Value;
            }
            return (max - min).ToString();
        }


        class Rule
        {
            public static HashSet<Rule> Rules = new HashSet<Rule>();

            public readonly string Name;
            public readonly string Result;
            public List<Rule> SubsequentRules = new List<Rule>();

            public Rule(string rule)
            {
                Name = rule.Substring(0, 2);
                Result = new String(new char[] { rule[0], rule[6], rule[1] });
                Rules.Add(this);
            }

            public static void MatchSubsequentRules()
            {
                foreach (Rule rule in Rules)
                {
                    rule.SubsequentRules.Add(Rules.Single(r => r.Name == rule.Result.Substring(0, 2)));
                    rule.SubsequentRules.Add(Rules.Single(r => r.Name == rule.Result.Substring(1, 2)));
                }
            }
        }

        public override string Part2()
        {
            string initialPolymer = "";
            using (var input = File.OpenText(InputFile))
            {
                initialPolymer = input.ReadLine();
                input.ReadLine();
                while (!input.EndOfStream) new Rule(input.ReadLine());
            }

            Rule.MatchSubsequentRules();

            Dictionary<Rule, Int64> ruleCount = new Dictionary<Rule, Int64>();
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
                Dictionary<Rule, Int64> newRuleCount = new Dictionary<Rule, Int64>();
                foreach (KeyValuePair<Rule, Int64> rule in ruleCount.Where(r => r.Value > 0))
                {
                    foreach (Rule nextRule in rule.Key.SubsequentRules)
                    {
                        if (!newRuleCount.ContainsKey(nextRule)) newRuleCount.Add(nextRule, 0);
                        newRuleCount[nextRule] += rule.Value;
                    }
                }
                ruleCount = newRuleCount;
            }

            Dictionary<char, Int64> chars = new Dictionary<char, Int64>();
            chars.Add(initialPolymer[0], 1);
            foreach (KeyValuePair<Rule, Int64> rule in ruleCount)
            {
                for (int i = 1; i < rule.Key.Result.Length; i++)
                {
                    if (!chars.ContainsKey(rule.Key.Result[i])) chars.Add(rule.Key.Result[i], 0);
                    chars[rule.Key.Result[i]] += rule.Value;
                }
            }

            Int64 max = 0;
            Int64 min = Int64.MaxValue;
            foreach (var n in chars)
            {
                if (n.Value > max) max = n.Value;
                if (n.Value < min) min = n.Value;
            }
            return (max - min).ToString();
        }
    }
}
