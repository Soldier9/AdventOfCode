namespace AdventOfCode.Solvers.Year2024
{
    class Day23Solver : AbstractSolver
    {
        Dictionary<string, HashSet<string>> Connections = [];

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split('-');
                    if (Connections.ContainsKey(line[0])) Connections[line[0]].Add(line[1]);
                    else Connections.Add(line[0], [line[1]]);
                    if (Connections.ContainsKey(line[1])) Connections[line[1]].Add(line[0]);
                    else Connections.Add(line[1], [line[0]]);
                }
            }

            HashSet<(string, string, string)> groupsOfThree = [];
            foreach (var c1 in Connections)
            {
                foreach (var c2 in c1.Value.Where(c2 => Connections[c2].Contains(c1.Key)).Select(c2 => (Key: c2, Value: Connections[c2])))
                {
                    foreach (var c3 in c1.Value.Where(c3 => Connections[c3].Contains(c1.Key) && Connections[c3].Contains(c2.Key)).Select(c3 => (Key: c3, Value: Connections[c3])))
                    {
                        var group = (new List<string>([c1.Key, c2.Key, c3.Key])).Order().ToArray();
                        groupsOfThree.Add((group[0], group[1], group[2]));
                    }
                }
            }

            int result = groupsOfThree.Count(g => g.Item1[0] == 't' || g.Item2[0] == 't' || g.Item3[0] == 't');
            return result.ToString();
        }

        public override string Part2()
        {
            HashSet<string> largestGroup = [];
            Queue<HashSet<string>> groupQueue = [];
            HashSet<string> isQueued = [];

            foreach (var c1 in Connections) groupQueue.Enqueue([c1.Key]);

            while(groupQueue.Count > 0)
            {
                var group = groupQueue.Dequeue();
                var c1 = group.First();
                foreach(var c2 in Connections[c1])
                {
                    if (IsInGroup(c2, group))
                    {
                        var newGroup = group.Union([c2]).ToHashSet();
                        var newGroupString = String.Join("", newGroup.Order());
                        if (isQueued.Contains(newGroupString)) continue;
                        groupQueue.Enqueue(newGroup);
                        isQueued.Add(newGroupString);
                    }
                }
                if(group.Count > largestGroup.Count) largestGroup = group;
            }
            return String.Join(",", largestGroup.Order()).ToString();
        }

        bool IsInGroup(string computer, HashSet<string> group)
        {
            if (Connections[computer].Intersect(group).Count() == group.Count) return true;
            return false;
        }
    }
}
