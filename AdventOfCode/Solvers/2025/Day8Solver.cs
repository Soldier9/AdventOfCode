namespace AdventOfCode.Solvers._2025;

internal class Day8Solver : AbstractSolver
{
    class Node
    {
        public static readonly HashSet<Node> AllNodes = [];
        public static readonly Dictionary<double, (Node a, Node b)> Distances = [];

        public readonly (int x, int y, int z) Position;
        public Circuit Circuit;

        public Node(string position)
        {
            var parts = position.Split(',');
            Position = (int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));

            foreach (var otherNode in AllNodes)
            {
                var distance = DistanceTo(otherNode);
                Distances.Add(distance, (this, otherNode));
            }

            Circuit = new();
            Circuit.Nodes.Add(this);

            AllNodes.Add(this);
        }

        private double DistanceTo(Node otherNode)
        {
            return Math.Sqrt(Math.Pow(otherNode.Position.x - this.Position.x, 2) +
                             Math.Pow(otherNode.Position.y - this.Position.y, 2) +
                             Math.Pow(otherNode.Position.z - this.Position.z, 2));
        }
    }

    class Circuit
    {
        public static readonly HashSet<Circuit> AllCircuits = [];

        public readonly HashSet<Node> Nodes = [];

        public Circuit()
        {
            AllCircuits.Add(this);
        }

        private void MergeCircuits(Circuit otherCircuit)
        {
            foreach (var node in otherCircuit.Nodes)
            {
                Nodes.Add(node);
                node.Circuit = this;
            }

            AllCircuits.Remove(otherCircuit);
        }

        public static void Connect((Node a, Node b) pair)
        {
            if (pair.a.Circuit == pair.b.Circuit) return;
            pair.a.Circuit.MergeCircuits(pair.b.Circuit);
        }
    }


    public override string Part1()
    {
        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var coordinates = input.ReadLine()!;
                _ = new Node(coordinates);
            }
        }

        var connectionsToCreate = 1000;
        foreach (var pair in Node.Distances.OrderBy(d => d.Key).Select(d => d.Value).Take(connectionsToCreate))
            Circuit.Connect(pair);

        var result = 1;

        foreach (var count in Circuit.AllCircuits.OrderByDescending(c => c.Nodes.Count).Select(c => c.Nodes.Count)
                     .Take(3)) result *= count;

        return result.ToString();
    }

    public override string Part2()
    {
        Node.AllNodes.Clear();
        Node.Distances.Clear();
        Circuit.AllCircuits.Clear();

        using (var input = File.OpenText(InputFile))
        {
            while (!input.EndOfStream)
            {
                var coordinates = input.ReadLine()!;
                _ = new Node(coordinates);
            }
        }

        var result = 0;
        foreach (var pair in Node.Distances.OrderBy(d => d.Key).Select(d => d.Value))
        {
            Circuit.Connect(pair);
            if (Circuit.AllCircuits.Count == 1)
            {
                result = pair.a.Position.x * pair.b.Position.x;
                break;
            }
        }

        return result.ToString();
    }
}