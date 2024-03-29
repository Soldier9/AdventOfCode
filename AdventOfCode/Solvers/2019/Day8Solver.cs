﻿using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2019
{
    class Day8Solver : AbstractSolver
    {
        public override string Part1()
        {
            string input = File.OpenText(InputFile).ReadLine()!;
            int width = 25;
            int height = 6;

            Dictionary<string, int> layers = new();
            for (int i = 0; i < input.Length; i += (width * height))
            {
                string layer = input.Substring(i, width * height);
                layers.Add(layer, Regex.Matches(layer, @"0").Count);
            }

            string relevantLayer = layers.OrderBy(l => l.Value).First().Key;

            int ones = Regex.Matches(relevantLayer, @"1").Count;
            int twos = Regex.Matches(relevantLayer, @"2").Count;

            return (ones * twos).ToString();
        }

        public override string Part2()
        {
            string input = File.OpenText(InputFile).ReadLine()!;
            int width = 25;
            int height = 6;

            List<string> layers = new();
            for (int i = 0; i < input.Length; i += (width * height))
            {
                layers.Add(input.Substring(i, width * height));
            }

            StringBuilder sb = new(new string('2', width * height));
            foreach (string layer in layers)
            {
                for (int i = 0; i < layer.Length; i++)
                {
                    if (sb[i] == '2') sb[i] = layer[i];
                }
            }

            string finalLayer = sb.ToString();
            for (int i = 0; i < width * height; i += width)
            {
                Console.WriteLine(finalLayer.Substring(i, width).Replace('0', ' '));    // Replace black for readability in ascii "white-on-black" :)
            }

            return "In the image";
        }
    }
}
