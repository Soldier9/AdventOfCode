using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day7Solver : AbstractSolver
    {
        class Bag
        {
            public string color;
            public Dictionary<string, int> content = new Dictionary<string, int>();
            public Dictionary<Bag, int> linkedContent = new Dictionary<Bag, int>();
        }

        private bool SearchForBag(string color, Dictionary<Bag, int> content)
        {
            foreach (var item in content)
            {
                if (item.Key.color == color) return true;
                if (SearchForBag(color, item.Key.linkedContent)) return true;
            }
            return false;
        }

        List<Bag> Bags = new List<Bag>();

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Bag newBag = new Bag();

                    string[] line = input.ReadLine().Split(new[] { " bags contain " }, StringSplitOptions.None);
                    newBag.color = line[0];

                    string[] content = line[1].Split(new[] { ", " }, StringSplitOptions.None);
                    if (content.Length == 1 && content[0] == "no other bags.")
                    {
                        Bags.Add(newBag);
                        continue;
                    }

                    foreach (string item in content)
                    {
                        string[] s = item.Split(new[] { ' ' }, 2);
                        s[1] = s[1].Replace(" bags", "");
                        s[1] = s[1].Replace(" bag", "");
                        s[1] = s[1].Replace(".", "");
                        newBag.content.Add(s[1], int.Parse(s[0]));
                    }
                    Bags.Add(newBag);
                }
            }

            foreach (Bag bag in Bags)
            {
                foreach (var item in bag.content)
                {
                    bag.linkedContent.Add(Bags.Find(b => b.color == item.Key), item.Value);
                }
            }

            int result = 0;
            foreach (Bag bag in Bags)
            {
                if (SearchForBag("shiny gold", bag.linkedContent)) result++;
            }
            return result.ToString();
        }

        private int CountBags(Dictionary<Bag, int> content)
        {
            int result = 0;
            foreach (var item in content)
            {
                result += item.Value * (1 + CountBags(item.Key.linkedContent));
            }
            return result;
        }

        public override string Part2()
        {
            return CountBags(Bags.Find(b => b.color == "shiny gold").linkedContent).ToString();
        }
    }
}