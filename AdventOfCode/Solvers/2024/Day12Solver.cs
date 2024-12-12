namespace AdventOfCode.Solvers.Year2024
{
    class Day12Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> Map = [];
        Dictionary<char, (int x, int y)> Directions = new()
        {
                { 'N', (0, -1) },
                { 'E', (1, 0) },
                { 'S', (0, 1) },
                { 'W', (-1, 0) }
            };

        Dictionary<int, HashSet<(int x, int y)>> Areas = [];

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = 0;
                    string line = input.ReadLine()!;
                    foreach (char c in line)
                    {
                        Map.Add((x++, y), c);
                    }
                    y++;
                }
            }

            int AreaNum = 0;
            HashSet<(int x, int y)> mappedPositions = [];
            foreach ((int x, int y) pos in Map.Keys)
            {
                if (mappedPositions.Contains(pos)) continue;
                Areas.Add(AreaNum, ConnectedPlots(pos));
                mappedPositions.UnionWith(Areas[AreaNum]);
                AreaNum++;
            }

            int result = 0;
            foreach (KeyValuePair<int, HashSet<(int x, int y)>> area in Areas)
            {
                int perimeter = 0;
                foreach ((int x, int y) plot in area.Value)
                {
                    foreach ((int x, int y) direction in Directions.Values)
                    {
                        (int x, int y) neighbor = AddPos(plot, direction);
                        if (!area.Value.Contains(neighbor)) perimeter++;
                    }
                }

                result += area.Value.Count * perimeter;
            }

            return result.ToString();
        }

        private HashSet<(int x, int y)> ConnectedPlots((int x, int y) pos, HashSet<(int x, int y)>? visited = null)
        {
            HashSet<(int x, int y)> result = [];
            visited ??= [];
            result.Add(pos);
            visited.Add(pos);

            foreach (KeyValuePair<char, (int x, int y)> direction in Directions)
            {
                (int x, int y) neighbor = AddPos(pos, direction.Value);
                if (Map.TryGetValue(neighbor, out char neighborPlot) && !visited.Contains(neighbor) && neighborPlot == Map[pos])
                {
                    result.UnionWith(ConnectedPlots(neighbor, visited));
                }
            }

            return result;
        }

        private (int x, int y) AddPos((int x, int y) pos1, (int x, int y) pos2)
        {
            return (pos1.x + pos2.x, pos1.y + pos2.y);
        }


        public override string Part2()
        {
            int result = 0;
            foreach (KeyValuePair<int, HashSet<(int x, int y)>> area in Areas)
            {
                int corners = 0;
                foreach ((int x, int y) plot in area.Value)
                {
                    corners += CornersForPlot(plot);
                }

                result += area.Value.Count * corners;
            }

            return result.ToString();
        }

        Dictionary<string, (int x, int y)> DiagonalDirections = new()
        {
                { "NE", (1, -1) },
                { "SE", (1, 1) },
                { "SW", (-1, 1) },
                { "NW", (-1, -1) }
            };
        private int CornersForPlot((int x, int y) pos)
        {
            int corners = 0;
            foreach (KeyValuePair<string, (int x, int y)> diag in DiagonalDirections)
            {
                (int x, int y) diagNeighbor = AddPos(pos, diag.Value);
                (int x, int y) neighbor1 = AddPos(pos, Directions[diag.Key[0]]);
                (int x, int y) neighbor2 = AddPos(pos, Directions[diag.Key[1]]);
                if (!Map.ContainsKey(diagNeighbor) || Map[pos] != Map[diagNeighbor])
                {
                    if (!Map.ContainsKey(neighbor1) && !Map.ContainsKey(neighbor2)) corners++;
                    else if (!Map.ContainsKey(neighbor1) && Map[pos] != Map[neighbor2]) corners++;
                    else if (!Map.ContainsKey(neighbor2) && Map[pos] != Map[neighbor1]) corners++;
                    else if (Map.ContainsKey(neighbor1) && Map.ContainsKey(neighbor2) && Map[neighbor1] != Map[pos] && Map[neighbor2] != Map[pos]) corners++;
                    else if (Map.ContainsKey(neighbor1) && Map.ContainsKey(neighbor2) && Map[neighbor1] == Map[neighbor2]) corners++;
                }
                else if (Map.ContainsKey(diagNeighbor) && Map.ContainsKey(neighbor1) && Map.ContainsKey(neighbor2) && Map[neighbor1] != Map[pos] && Map[neighbor2] != Map[pos]) corners++;
            }
            return corners;
        }
    }
}
