using System.Text;

namespace AdventOfCode.Solvers.Year2024
{
    class Day21Solver : AbstractSolver
    {
        abstract class KeyPad
        {
            protected Dictionary<char, (int x, int y)> Buttons = [];
            protected Dictionary<(int x, int y), char> Position2Button = [];
            Dictionary<(int x, int y), char> Directions = new()
            {
                { (1,0),  '>' },
                { (-1,0), '<' },
                { (0,-1), '^' },
                { (0,1),  'v' }
            };
            protected (int x, int y) CurrentPosition;


            Dictionary<((int x, int y) from, (int x, int y) to, (int x, int y)[] pathSoFar), List<List<(int x, int y)>>> CachedShortestPathsToDestination = [];
            List<List<(int x, int y)>> ShortestPathsToDestination((int x, int y) from, (int x, int y) to, List<(int x, int y)> pathSoFar)
            {
                (int x, int y)[] pathSoFarArray = pathSoFar.ToArray();
                if (CachedShortestPathsToDestination.TryGetValue((from, to, pathSoFarArray), out List<List<(int x, int y)>>? result)) return result!;
                else result = new();
                List<(int x, int y)> recPathSoFar = new(pathSoFar);
                recPathSoFar.Add(from);
                if (from == to) return [recPathSoFar];

                foreach (var nextPosition in Directions.Select(d => AddPos(d.Key, from)).Where(p => Position2Button.ContainsKey(p) && !pathSoFar.Contains(p)))
                {
                    result.AddRange(ShortestPathsToDestination(nextPosition, to, recPathSoFar));
                }
                if (result.Count > 1)
                {
                    int shortestPath = result.MinBy(r => r.Count)!.Count;
                    result.RemoveAll(r => r.Count > shortestPath);
                }
                CachedShortestPathsToDestination.Add((from, to, pathSoFarArray), result);
                return result;
            }

            Dictionary<string, List<string>> CachedKeyPressesForCombination = [];
            public List<string> KeyPressesForCombination(string combination)
            {
                if (CachedKeyPressesForCombination.TryGetValue(combination, out List<string>? results)) return results;
                else results = [""];
                foreach (char keyPress in combination)
                {
                    List<string> nextResults = [];
                    List<List<(int x, int y)>> possiblePaths = ShortestPathsToDestination(CurrentPosition, Buttons[keyPress], []);
                    foreach (string possiblePath in FilterBestResults(possiblePaths.Select(p => Path2KeySequence(p)))) { 
                        foreach (string result in results) { 
                            nextResults.Add(result + possiblePath + "A");
                        }
                    }
                    results = nextResults;
                    CurrentPosition = Buttons[keyPress];
                }
                CachedKeyPressesForCombination.Add(combination, results);
                return results;
            }

            List<string> FilterBestResults(IEnumerable<string> results)
            {
                int bestResultCost = int.MaxValue;
                Dictionary<string, int> bestResults = [];
                foreach (string result in results)
                {
                    int pathCost = PathCost(result);
                    bestResults.Add(result, pathCost);
                    bestResultCost = Math.Min(bestResultCost, pathCost);
                }
                bestResults = bestResults.Where(r => r.Value == bestResultCost).ToDictionary();
                return bestResults.Select(r => r.Key).ToList();
            }

            string Path2KeySequence(List<(int x, int y)> path)
            {
                StringBuilder keySequence = new StringBuilder();
                for (int i = 1; i < path.Count; i++)
                {
                    char keyPressed = Directions[GetDirection(path[i - 1], path[i])];
                    keySequence.Append(keyPressed);
                }
                return keySequence.ToString();
            }

            static int PathCost(string keySequence)
            {
                int cost = 0;
                char? lastChar = null;
                foreach (var keyPress in keySequence)
                {
                    if (lastChar != null)
                    {
                        if (keyPress == lastChar) cost += 1;
                        else cost += 10;
                    }
                    lastChar = keyPress;
                }
                return cost;
            }

            private (int x, int y) AddPos((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
            private (int x, int y) GetDirection((int x, int y) from, (int x, int y) to) => (to.x - from.x, to.y - from.y);
        }


        class NumPad : KeyPad
        {
            public NumPad()
            {
                Buttons = new()
                {
                    { '7', (0, 0) },
                    { '8', (1, 0) },
                    { '9', (2, 0) },
                    { '4', (0, 1) },
                    { '5', (1, 1) },
                    { '6', (2, 1) },
                    { '1', (0, 2) },
                    { '2', (1, 2) },
                    { '3', (2, 2) },
                    { '0', (1, 3) },
                    { 'A', (2, 3) }
                };
                Position2Button = new()
                {
                    { (0, 0), '7' },
                    { (1, 0), '8' },
                    { (2, 0), '9' },
                    { (0, 1), '4' },
                    { (1, 1), '5' },
                    { (2, 1), '6' },
                    { (0, 2), '1' },
                    { (1, 2), '2' },
                    { (2, 2), '3' },
                    { (1, 3), '0' },
                    { (2, 3), 'A' }
                };
                CurrentPosition = Buttons['A'];
            }
        }

        class ArrowPad : KeyPad
        {
            public ArrowPad()
            {
                Buttons = new()
                {
                    { '^', (1, 0) },
                    { 'A', (2, 0) },
                    { '<', (0, 1) },
                    { 'v', (1, 1) },
                    { '>', (2, 1) }
                };
                Position2Button = new()
                {
                    { (1, 0), '^' },
                    { (2, 0), 'A' },
                    { (0, 1), '<' },
                    { (1, 1), 'v' },
                    { (2, 1), '>' }
                };
                CurrentPosition = Buttons['A'];
            }

        }

        List<string> Combinations = [];

        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Combinations.Add(input.ReadLine()!);
                }
            }

            long result = 0;
            foreach (string combination in Combinations)
            {
                NumPad numPad = new NumPad();
                ArrowPad arrowPad = new ArrowPad();
                var keySequences = numPad.KeyPressesForCombination(combination);
                int shortestSequence = keySequences.MinBy(k => k.Length)!.Length;
                keySequences.RemoveAll(k => k.Length > shortestSequence);

                List<string> nextKeysequences = [];
                for (int i = 0; i < 2; i++)
                {
                    nextKeysequences = [];
                    foreach (var keySequence in keySequences) nextKeysequences.AddRange(arrowPad.KeyPressesForCombination(keySequence));
                    keySequences = nextKeysequences;
                    shortestSequence = keySequences.MinBy(k => k.Length)!.Length;
                    keySequences.RemoveAll(k => k.Length > shortestSequence);
                }

                result += keySequences[0].Length * int.Parse(combination[..^1]);
            }
            return result.ToString();
        }

        public override string Part2()
        {
            return "Too slow";
            //long result = 0;
            //foreach (string combination in Combinations)
            //{
            //    NumPad numPad = new NumPad();
            //    ArrowPad arrowPad = new ArrowPad();
            //    var keySequences = numPad.KeyPressesForCombination(combination);
            //    int shortestSequence = keySequences.MinBy(k => k.Length)!.Length;
            //    keySequences.RemoveAll(k => k.Length > shortestSequence);

            //    List<string> nextKeysequences = [];
            //    for (int i = 0; i < 3; i++)
            //    {
            //        nextKeysequences = [];
            //        foreach (var keySequence in keySequences) nextKeysequences.AddRange(arrowPad.KeyPressesForCombination(keySequence));
            //        keySequences = nextKeysequences;
            //        shortestSequence = keySequences.MinBy(k => k.Length)!.Length;
            //        keySequences.RemoveAll(k => k.Length > shortestSequence);
            //    }

            //    result += keySequences[0].Length * int.Parse(combination[..^1]);
            //}
            //return result.ToString();
        }
    }
}
