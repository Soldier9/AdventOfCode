using System.Text;

namespace AdventOfCode.Solvers.Year2024
{
    class Day15Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> Map = [];
        (int x, int y) RobotPos;
        string Moves = "";

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                StringBuilder sb = new();
                bool parsingMap = true;
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (parsingMap)
                    {
                        if (line.Length == 0)
                        {
                            parsingMap = false;
                            continue;
                        }
                        int x = 0;
                        foreach (char c in line)
                        {
                            if (c != '.') Map.Add((x, y), c);
                            if (c == '@') RobotPos = (x, y);
                            x++;
                        }
                        y++;
                    }
                    else
                    {
                        sb.Append(line);
                    }
                }
                Moves = sb.ToString();
            }

            foreach (char move in Moves) RobotPos = Move(RobotPos, move);

            int result = 0;
            foreach (KeyValuePair<(int x, int y), char> position in Map)
            {
                if (position.Value == 'O') result += position.Key.x + position.Key.y * 100;
            }
            return result.ToString();
        }

        private (int x, int y) Move((int x, int y) pos, char direction)
        {
            (int x, int y) newPos = direction switch
            {
                '>' => (pos.x + 1, pos.y),
                'v' => (pos.x, pos.y + 1),
                '<' => (pos.x - 1, pos.y),
                '^' => (pos.x, pos.y - 1),
                _ => pos
            };

            if (Map.TryGetValue(newPos, out char adjacentObject) && (adjacentObject == '#' || Move(newPos, direction) == newPos)) return pos;
            Map.Add(newPos, Map[pos]);
            Map.Remove(pos);
            return newPos;
        }


        public override string Part2()
        {
            Map = [];
            using (StreamReader input = File.OpenText(InputFile))
            {
                StringBuilder sb = new();
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Length == 0) break;

                    int x = 0;
                    foreach (char c in line)
                    {
                        switch (c)
                        {
                            case '#':
                                Map.Add((x, y), c);
                                Map.Add((x + 1, y), c);
                                break;
                            case 'O':
                                Map.Add((x, y), '[');
                                Map.Add((x + 1, y), ']');
                                break;
                            case '@':
                                Map.Add((x, y), c);
                                RobotPos = (x, y);
                                break;
                        }
                        x += 2;
                    }
                    y++;
                }
            }

            if (Program.VisualizationEnabled) Program.PrintData(Program.CreateStringFromDict(Map), 0);

            foreach (char move in Moves)
            {
                RobotPos = Move2(RobotPos, move);
                if(Program.VisualizationEnabled) Program.PrintData(Program.CreateStringFromDict(Map), 0);
            }

            int result = 0;
            foreach (KeyValuePair<(int x, int y), char> position in Map)
            {
                if (position.Value == '[') result += position.Key.x + position.Key.y * 100;
            }
            return result.ToString();
        }


        private (int x, int y) Move2((int x, int y) pos, char direction, bool testMode = false)
        {
            (int x, int y) newPos = direction switch
            {
                '>' => (pos.x + 1, pos.y),
                'v' => (pos.x, pos.y + 1),
                '<' => (pos.x - 1, pos.y),
                '^' => (pos.x, pos.y - 1),
                _ => pos
            };

            if (Map.TryGetValue(newPos, out char adjacentObject))
            {
                List<(int x, int y)> movesToTest = [newPos];
                switch (adjacentObject)
                {
                    case '#': return pos;
                    case '[': if (direction == 'v' || direction == '^') movesToTest.Add((newPos.x + 1, newPos.y)); break;
                    case ']': if (direction == 'v' || direction == '^') movesToTest.Add((newPos.x - 1, newPos.y)); break;
                }

                bool allMovesPossible = true;
                foreach ((int x, int y) testPos in movesToTest)
                {
                    if (Move2(testPos, direction, true) == testPos)
                    {
                        allMovesPossible = false;
                        break;
                    }
                }
                if (!allMovesPossible) return pos;

                if (!testMode) foreach ((int x, int y) testPos in movesToTest) Move2(testPos, direction);
            }
            if (!testMode)
            {
                Map.Add(newPos, Map[pos]);
                Map.Remove(pos);
            }
            return newPos;
        }
    }
}
