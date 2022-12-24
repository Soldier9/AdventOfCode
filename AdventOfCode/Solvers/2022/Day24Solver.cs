using System.Collections;
using System.Collections.Generic;
using System.Security;

namespace AdventOfCode.Solvers.Year2022
{
    class Day24Solver : AbstractSolver
    {
        Dictionary<(int x, int y), List<char>> startMap = new();
        List<Dictionary<(int x, int y), List<char>>> mapStates = new();
        int possibleStates = 0;

        (int x, int y) startPos = (0, 0);
        (int x, int y) endPos = (0, 0);
        int width = -1;
        int height = -2;
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach (char c in line)
                    {
                        if (c != '.') startMap.Add((x, y), new List<char> { c });
                        else if (startPos == (0, 0)) startPos = (x, y);
                        else endPos = (x, y);
                        x++;
                    }
                    y++;
                    height++;
                    if (width == -1) width = line.Length - 2;
                }
            }

            possibleStates = lcm(width, height);
            mapStates.Add(startMap);
            for (int i = 0; i < possibleStates; i++)
            {
                Dictionary<(int x, int y), List<char>> nextMap = new();
                foreach (KeyValuePair<(int x, int y), List<char>> point in mapStates[i])
                {
                    foreach (char c in point.Value)
                    {
                        (int x, int y) nextPos = c switch
                        {
                            '>' => (point.Key.x + 1, point.Key.y),
                            '<' => (point.Key.x - 1, point.Key.y),
                            '^' => (point.Key.x, point.Key.y - 1),
                            'v' => (point.Key.x, point.Key.y + 1),
                            _ => point.Key,
                        };
                        if (mapStates[i].ContainsKey(nextPos) && mapStates[i][nextPos].Contains('#'))
                        {
                            switch (c)
                            {
                                case '>': nextPos = (mapStates[i].Where(b => b.Value.Contains('#')).MinBy(b => b.Key.x).Key.x + 1, point.Key.y); break;
                                case '<': nextPos = (mapStates[i].Where(b => b.Value.Contains('#')).MaxBy(b => b.Key.x).Key.x - 1, point.Key.y); break;
                                case '^': nextPos = (point.Key.x, mapStates[i].Where(b => b.Value.Contains('#')).MaxBy(b => b.Key.y).Key.y - 1); break;
                                case 'v': nextPos = (point.Key.x, mapStates[i].Where(b => b.Value.Contains('#')).MinBy(b => b.Key.y).Key.y + 1); break;
                            }
                        }

                        if (!nextMap.ContainsKey(nextPos)) nextMap.Add(nextPos, new List<char> { c });
                        else nextMap[nextPos].Add(c);
                    }
                }

                mapStates.Add(nextMap);
            }

            PriorityQueue<(int mapState, (int x, int y) pos), int> queue = new();
            queue.Enqueue((0, startPos), 0);
            while (queue.Count > 0) NextMove(endPos, queue);

            return BestResult.ToString()!;
        }

        int BestResult = int.MaxValue;
        int EndMapState = 0;
        HashSet<(int mapState, (int x, int y) pos)> VisitedStates = new();
        public void NextMove((int x, int y) target, PriorityQueue<(int mapState, (int x, int y) pos), int> queue)
        {
            _ = queue.TryDequeue(out (int mapState, (int x, int y) pos) current, out int mins);
            
            if (VisitedStates.Contains(current)) return;

            if (current.pos == target)
            {
                if(mins < BestResult)
                {
                    BestResult = mins;
                    EndMapState = current.mapState;
                }
                return;
            }

            int nextMapState = (current.mapState + 1) % possibleStates;
            if (!mapStates[nextMapState].ContainsKey((current.pos.x + 1, current.pos.y))) queue.Enqueue((nextMapState, (current.pos.x + 1, current.pos.y)), mins + 1);
            if (!mapStates[nextMapState].ContainsKey((current.pos.x - 1, current.pos.y))) queue.Enqueue((nextMapState, (current.pos.x - 1, current.pos.y)), mins + 1);
            if (current.pos.y > 0 && !mapStates[nextMapState].ContainsKey((current.pos.x, current.pos.y - 1))) queue.Enqueue((nextMapState, (current.pos.x, current.pos.y - 1)), mins + 1);
            if (current.pos.y <= height && !mapStates[nextMapState].ContainsKey((current.pos.x, current.pos.y + 1))) queue.Enqueue((nextMapState, (current.pos.x, current.pos.y + 1)), mins + 1);
            if (!mapStates[nextMapState].ContainsKey(current.pos)) queue.Enqueue((nextMapState, current.pos), mins + 1);

            _ = VisitedStates.Add(current);
            return;
        }

        public override string Part2()
        {
            PriorityQueue<(int mapState, (int x, int y) pos), int> queue = new();
            queue.Enqueue((EndMapState, endPos), BestResult);
            VisitedStates.Clear();
            BestResult = int.MaxValue;
            while (queue.Count > 0) NextMove(startPos, queue);

            queue.Enqueue((EndMapState, startPos), BestResult);
            VisitedStates.Clear();
            BestResult = int.MaxValue;
            while (queue.Count > 0) NextMove(endPos, queue);

            return BestResult.ToString()!;
        }

        private static int lcm(int x, int y)
        {
            return (x * y) / gcd(x, y);
        }

        private static int gcd(int x, int y)
        {
            int result = Math.Min(x, y);
            while (result > 0)
            {
                if (x % result == 0 && y % result == 0) break;
                result--;
            }
            return result;
        }
    }
}
