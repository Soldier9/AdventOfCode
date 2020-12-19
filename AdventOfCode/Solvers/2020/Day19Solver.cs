﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day19Solver : AbstractSolver
    {
        class Rule
        {
            public static List<Rule> AllRules = new List<Rule>();

            public int Number;
            HashSet<string> Patterns = new HashSet<string>();
            public List<List<int>> ReferencedRules = new List<List<int>>();

            public Rule(string input)
            {
                Number = int.Parse(input.Split(':')[0]);

                string[] ruleSets = input.Split(':')[1].Split('|');
                for (int i = 0; i < ruleSets.Length; i++)
                {
                    foreach (string item in ruleSets[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        int ruleRefNum;
                        if (int.TryParse(item, out ruleRefNum))
                        {
                            if (ReferencedRules.Count <= i) ReferencedRules.Add(new List<int>());
                            ReferencedRules[i].Add(ruleRefNum);
                        }
                        else
                        {
                            Patterns.Add(item.Replace("\"", ""));
                        }
                    }
                }
            }

            public HashSet<string> GetPatterns()
            {
                if (ReferencedRules.Count > 0)
                {
                    for (int i = 0; i < ReferencedRules.Count; i++)
                    {
                        List<string> newPatterns = new List<string>();
                        newPatterns.Add("");
                        foreach (int refRule in ReferencedRules[i])
                        {
                            int newPatternCount = newPatterns.Count;
                            Rule referencedRule = AllRules.Where(r => r.Number == refRule).Single();
                            foreach (string pattern in referencedRule.GetPatterns())
                            {
                                for (int j = 0; j < newPatternCount; j++)
                                {
                                    newPatterns.Add(newPatterns[j] + pattern);
                                }
                            }
                            newPatterns.RemoveRange(0, newPatternCount);
                        }
                        foreach(string pattern in newPatterns)
                        {
                            Patterns.Add(pattern);
                        }
                    }
                    ReferencedRules.Clear();
                }
                return Patterns;
            }
        }


        public override string Part1()
        {
            List<string> messages = new List<string>();

            using (var input = File.OpenText(InputFile))
            {
                bool parsingRules = true;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.Length == 0)
                    {
                        parsingRules = false;
                        continue;
                    }

                    if (parsingRules)
                    {
                        Rule.AllRules.Add(new Rule(line));
                    }
                    else
                    {
                        messages.Add(line);
                    }
                }
            }

            int result = 0;
            Rule rule0 = Rule.AllRules.Where(r => r.Number == 0).Single();
            foreach(string message in messages)
            {
                if (rule0.GetPatterns().Contains(message)) result++;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            Rule.AllRules.Clear();
            List<string> messages = new List<string>();

            using (var input = File.OpenText(InputFile))
            {
                bool parsingRules = true;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.Length == 0)
                    {
                        parsingRules = false;
                        continue;
                    }

                    if (parsingRules)
                    {
                        if (line.StartsWith("8:")) line = "8: 42 | 42 8";
                        else if(line.StartsWith("11:")) line = "11: 42 31 | 42 11 31";

                        Rule.AllRules.Add(new Rule(line));
                    }
                    else
                    {
                        messages.Add(line);
                    }
                }
            }

            Rule rule42 = Rule.AllRules.Where(r => r.Number == 42).Single();
            string regexPattern42 = "(";
            foreach (string pattern in rule42.GetPatterns())
            {
                regexPattern42 += pattern + "|";
            }
            regexPattern42 = regexPattern42.Substring(0, regexPattern42.Length - 1) + ")";

            Rule rule31 = Rule.AllRules.Where(r => r.Number == 31).Single();
            string regexPattern31 = "(";
            foreach (string pattern in rule31.GetPatterns())
            {
                regexPattern31 += pattern + "|";
            }
            regexPattern31 = regexPattern31.Substring(0, regexPattern31.Length - 1) + ")";

            string regexPattern8 = regexPattern42 + "+";
            string regexPattern11 = regexPattern42 + "{n}" + regexPattern31 + "{n}";
            string regexPattern0 = "^" + regexPattern8 + regexPattern11 + "$";

            int result = 0;
            foreach(string msg in messages)
            {
                int quantifiersToTry = (msg.Length - 5) / 2;
                for(int i = 1; i < quantifiersToTry; i++)
                {
                    Regex r0 = new Regex(regexPattern0.Replace("{n}", "{" + i.ToString() + "}"));
                    if(r0.IsMatch(msg))
                    {
                        result++;
                        break;
                    }
                }   
            }

            return result.ToString();
        }
    }
}
