using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.Solvers.Year2020
{
    class Day3Solver : AbstractSolver
    {
        private List<string> map = new List<string>();

        private int FindTreesInTraversal(int right, int down) 
        {
            int trees = 0;

            int height = map.Count;
            int width = map[0].Length;
            
            int x = 0;
            int y = 0;

            while (true)
            {
                x += right;
                y += down;
                if (y >= height) break;
                if (x >= width) x -= width;

                if (map[y][x] == '#') trees++;
            }
            return trees;
        }

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    map.Add(input.ReadLine());
                }
            }

            return FindTreesInTraversal(3,1).ToString();
        }

        public override string Part2()
        {
            Int64 result = 1;
            result *= FindTreesInTraversal(1, 1);
            result *= FindTreesInTraversal(3, 1);
            result *= FindTreesInTraversal(5, 1);
            result *= FindTreesInTraversal(7, 1);
            result *= FindTreesInTraversal(1, 2);

            return result.ToString();
        }
    }
}
