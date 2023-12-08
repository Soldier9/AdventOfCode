using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day8Solver : AbstractSolver
    {
        List<char> instructions = new();

        class Node
        {
            public static Dictionary<string, Node> allNodes = new();

            public string Name;
            public Node? Left;
            public Node? Right;
            private string leftName;
            private string rightName;

            public Node(string name, string left, string right)
            {
                Name = name;
                leftName = left;
                rightName = right;

                allNodes.Add(name, this);
            }

            public static void MatchNodes()
            {
                foreach(KeyValuePair<string, Node> node in allNodes)
                {
                    node.Value.Left = allNodes[node.Value.leftName];
                    node.Value.Right = allNodes[node.Value.rightName];
                }
            }
        }
        public override string Part1()
        {
            Regex nameParser = new Regex(@"\w{3}");
            using (StreamReader input = File.OpenText(InputFile))
            {
                string line = input.ReadLine()!;
                foreach(char c in line) { instructions.Add(c); }

                while (!input.EndOfStream)
                {
                    line = input.ReadLine()!;
                    if(line.Length > 0) { 
                        MatchCollection matches = nameParser.Matches(line);
                        _ = new Node(matches[0].Value, matches[1].Value, matches[2].Value);
                    }
                }
            }
            Node.MatchNodes();

            Node currentNode = Node.allNodes["AAA"];
            int inst = 0;

            while (currentNode!.Name != "ZZZ")
            {
                switch (instructions[inst % instructions.Count])
                {
                    case 'L': currentNode = currentNode.Left!; break;
                    case 'R': currentNode = currentNode.Right!; break;
                }
                inst++;
            }

            return inst.ToString();
        }

        public override string Part2()
        {
            List<Node> currentNodes = new();
            currentNodes.AddRange(Node.allNodes.Where(n => n.Value.Name.EndsWith('A')).Select(n => n.Value));

            long result = 1;
            for (int i = 0; i < currentNodes.Count; i++)
            {
                long inst = 0;
                while (!currentNodes[i].Name.EndsWith('Z'))
                {
                    switch (instructions[(int)(inst % instructions.Count)])
                    {
                        case 'L': currentNodes[i] = currentNodes[i].Left!; break;
                        case 'R': currentNodes[i] = currentNodes[i].Right!; break;
                    }
                    inst++;
                }
                result = lcm(result, inst);
            }

            return result.ToString();
        }

        private static long lcm(long x, long y)
        {
            return (x * y) / gcd(x, y);
        }

        private static long gcd(long x, long y)
        {
            long result = Math.Min(x, y);
            while (result > 0)
            {
                if (x % result == 0 && y % result == 0) break;
                result--;
            }
            return result;
        }
    }
}
