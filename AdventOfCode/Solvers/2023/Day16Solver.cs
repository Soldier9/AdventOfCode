namespace AdventOfCode.Solvers.Year2023
{
    class Day16Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> grid = new();
        HashSet<(int x, int y)> light = new();
        HashSet<(int x, int y, int direction)> visited = new();

        int height = -1;
        int width = -1;
        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    for (int x = 0; x < line.Length; x++)
                    {
                        if (line[x] != '.') grid.Add((x, y), line[x]);
                    }
                    y++;
                }

                height = grid.Max(g => g.Key.x) + 1;
                width = grid.Max(g => g.Key.y) + 1;
            }

            return ProcessLight((-1, 0), 'e').ToString();
        }

        public int ProcessLight((int x, int y) pos, char direction)
        {
            if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            {
                light = new();
                visited = new();
            }
            if (visited.Contains((pos.x, pos.y, direction))) return light.Count;
            visited.Add((pos.x, pos.y, direction));

            (int x, int y) nextPos = pos;
            do
            {
                switch (direction)
                {
                    case 'n': nextPos = (nextPos.x, nextPos.y - 1); break;
                    case 's': nextPos = (nextPos.x, nextPos.y + 1); break;
                    case 'e': nextPos = (nextPos.x + 1, nextPos.y); break;
                    case 'w': nextPos = (nextPos.x - 1, nextPos.y); break;
                }
                if (nextPos.x < 0 || nextPos.x >= width || nextPos.y < 0 || nextPos.y >= height) return light.Count;
                light.Add(nextPos);
            } while (!grid.ContainsKey(nextPos));

            char nextDir = ' ';
            switch (grid[nextPos])
            {
                case '/':
                    switch (direction)
                    {
                        case 'n': nextDir = 'e'; break;
                        case 's': nextDir = 'w'; break;
                        case 'e': nextDir = 'n'; break;
                        case 'w': nextDir = 's'; break;
                    }
                    ProcessLight(nextPos, nextDir);
                    break;
                case '\\':
                    switch (direction)
                    {
                        case 'n': nextDir = 'w'; break;
                        case 's': nextDir = 'e'; break;
                        case 'e': nextDir = 's'; break;
                        case 'w': nextDir = 'n'; break;
                    }
                    ProcessLight(nextPos, nextDir);
                    break;
                case '|':
                    if("ns".Contains(direction)) ProcessLight(nextPos, direction);
                    else
                    {
                        ProcessLight(nextPos, 'n');
                        ProcessLight(nextPos, 's');
                    }
                    break;
                case '-':
                    if ("ew".Contains(direction)) ProcessLight(nextPos, direction);
                    else
                    {
                        ProcessLight(nextPos, 'e');
                        ProcessLight(nextPos, 'w');
                    }
                    break;
            }
            return light.Count;
        }

        public override string Part2()
        {
            int result = 0;
            for (int x = 0; x < width; x++)
            {
                result = Math.Max(result, ProcessLight((x, -1), 's'));
                result = Math.Max(result, ProcessLight((x, height), 'n'));
            }
            for (int y = 0; y < height; y++)
            {
                result = Math.Max(result, ProcessLight((-1, y), 'e'));
                result = Math.Max(result, ProcessLight((width, y), 'w'));
            }
            return result.ToString();
        }
    }
}
