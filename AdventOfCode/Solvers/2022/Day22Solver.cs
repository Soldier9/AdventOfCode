using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day22Solver : AbstractSolver
    {
        public override bool HasVisualization => true;

        class Tile
        {
            public static Dictionary<(int x, int y), Tile> map = new();

            public (int x, int y) pos;
            public char type;

            public Tile? up, down, left, right;
            public char? newDirUp, newDirDown, newDirLeft, newDirRight;

            public Tile((int x, int y) pos, char type)
            {
                this.pos = pos;
                this.type = type;

                map.Add(pos, this);
            }

            public static void SetupLinks()
            {
                foreach (Tile currentTile in map.Values)
                {
                    if (map.ContainsKey((currentTile.pos.x, currentTile.pos.y - 1))) currentTile.up = map[(currentTile.pos.x, currentTile.pos.y - 1)];
                    else currentTile.up = map[(currentTile.pos.x, map.Where(t => t.Key.x == currentTile.pos.x).MaxBy(t => t.Key.y).Key.y)];

                    if (map.ContainsKey((currentTile.pos.x, currentTile.pos.y + 1))) currentTile.down = map[(currentTile.pos.x, currentTile.pos.y + 1)];
                    else currentTile.down = map[(currentTile.pos.x, map.Where(t => t.Key.x == currentTile.pos.x).MinBy(t => t.Key.y).Key.y)];

                    if (map.ContainsKey((currentTile.pos.x - 1, currentTile.pos.y))) currentTile.left = map[(currentTile.pos.x - 1, currentTile.pos.y)];
                    else currentTile.left = map[(map.Where(t => t.Key.y == currentTile.pos.y).MaxBy(t => t.Key.x).Key.x, currentTile.pos.y)];

                    if (map.ContainsKey((currentTile.pos.x + 1, currentTile.pos.y))) currentTile.right = map[(currentTile.pos.x + 1, currentTile.pos.y)];
                    else currentTile.right = map[(map.Where(t => t.Key.y == currentTile.pos.y).MinBy(t => t.Key.x).Key.x, currentTile.pos.y)];
                }
            }

            public static void SetupCubeLinks()
            {
                foreach (Tile currentTile in map.Values)
                {
                    if (map.ContainsKey((currentTile.pos.x, currentTile.pos.y - 1))) currentTile.up = map[(currentTile.pos.x, currentTile.pos.y - 1)];
                    else
                    {
                        if (currentTile.pos.x <= 50)
                        {
                            int newY = 50 + currentTile.pos.x;
                            int newX = 51;
                            currentTile.up = map[(newX, newY)];
                            currentTile.newDirUp = 'R';
                        }
                        else if (currentTile.pos.x <= 100)
                        {
                            int newY = 150 + currentTile.pos.x - 50;
                            int newX = 1;
                            currentTile.up = map[(newX, newY)];
                            currentTile.newDirUp = 'R';
                        }
                        else
                        {
                            int newY = 200;
                            int newX = currentTile.pos.x - 100;
                            currentTile.up = map[(newX, newY)];
                            currentTile.newDirUp = 'U';
                        }
                    }

                    if (map.ContainsKey((currentTile.pos.x, currentTile.pos.y + 1))) currentTile.down = map[(currentTile.pos.x, currentTile.pos.y + 1)];
                    else
                    {
                        if (currentTile.pos.x <= 50)
                        {
                            int newY = 1;
                            int newX = currentTile.pos.x + 100;
                            currentTile.down = map[(newX, newY)];
                            currentTile.newDirDown = 'D';
                        }
                        else if (currentTile.pos.x <= 100)
                        {
                            int newY = currentTile.pos.x + 100;
                            int newX = 50;
                            currentTile.down = map[(newX, newY)];
                            currentTile.newDirDown = 'L';
                        }
                        else
                        {
                            int newY = currentTile.pos.x - 50;
                            int newX = 100;
                            currentTile.down = map[(newX, newY)];
                            currentTile.newDirDown = 'L';
                        }
                    }

                    if (map.ContainsKey((currentTile.pos.x - 1, currentTile.pos.y))) currentTile.left = map[(currentTile.pos.x - 1, currentTile.pos.y)];
                    else
                    {
                        if (currentTile.pos.y <= 50)
                        {
                            int newY = 101 + (50 - currentTile.pos.y);
                            int newX = 1;
                            currentTile.left = map[(newX, newY)];
                            currentTile.newDirLeft = 'R';
                        }
                        else if (currentTile.pos.y <= 100)
                        {
                            int newY = 101;
                            int newX = currentTile.pos.y - 50;
                            currentTile.left = map[(newX, newY)];
                            currentTile.newDirLeft = 'D';
                        }
                        else if (currentTile.pos.y <= 150)
                        {
                            int newY = 151 - currentTile.pos.y;
                            int newX = 51;
                            currentTile.left = map[(newX, newY)];
                            currentTile.newDirLeft = 'R';
                        }
                        else
                        {
                            int newY = 1;
                            int newX = currentTile.pos.y - 150 + 50;
                            currentTile.left = map[(newX, newY)];
                            currentTile.newDirLeft = 'D';
                        }
                    }

                    if (map.ContainsKey((currentTile.pos.x + 1, currentTile.pos.y))) currentTile.right = map[(currentTile.pos.x + 1, currentTile.pos.y)];
                    else
                    {
                        if (currentTile.pos.y <= 50)
                        {
                            int newY = 101 + (50 - currentTile.pos.y);
                            int newX = 100;
                            currentTile.right = map[(newX, newY)];
                            currentTile.newDirRight = 'L';
                        }
                        else if (currentTile.pos.y <= 100)
                        {
                            int newY = 50;
                            int newX = currentTile.pos.y - 50 + 100;
                            currentTile.right = map[(newX, newY)];
                            currentTile.newDirRight = 'U';
                        }
                        else if (currentTile.pos.y <= 150)
                        {
                            int newY = 51 - (currentTile.pos.y - 100);
                            int newX = 150;
                            currentTile.right = map[(newX, newY)];
                            currentTile.newDirRight = 'L';
                        }
                        else
                        {
                            int newY = 150;
                            int newX = currentTile.pos.y - 150 + 50;
                            currentTile.right = map[(newX, newY)];
                            currentTile.newDirRight = 'U';
                        }
                    }
                }
            }
        }

        List<string> moves = new();
        (int x, int y) startPos = (0, 0);

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingMap = true;
                int y = 1;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line == "")
                    {
                        parsingMap = false;
                        continue;
                    }
                    if (parsingMap)
                    {
                        int x = 0;
                        foreach (char c in line)
                        {
                            x++;
                            if (c == ' ') continue;
                            if (c == '.' && startPos == (0, 0)) startPos = (x, y);
                            new Tile((x, y), c);
                        }
                        y++;
                    }
                    else
                    {
                        Regex moveParser = new(@"(\d+|[LR])");
                        MatchCollection mc = moveParser.Matches(line);
                        foreach (Match m in mc) moves.Add(m.Value);
                    }
                }
            }

            Tile.SetupLinks();
            Tile currentTile = Tile.map[startPos];
            char direction = 'R';
            if (Program.VisualizationEnabled) PrintMap(currentTile, direction);

            foreach (string move in moves)
            {
                int len = 0;
                if (int.TryParse(move, out len))
                {
                    while (len-- > 0)
                    {
                        switch (direction)
                        {
                            case 'R':
                                if (currentTile.right!.type == '.') currentTile = currentTile.right;
                                else len = 0;
                                break;
                            case 'L':
                                if (currentTile.left!.type == '.') currentTile = currentTile.left;
                                else len = 0;
                                break;
                            case 'U':
                                if (currentTile.up!.type == '.') currentTile = currentTile.up;
                                else len = 0;
                                break;
                            case 'D':
                                if (currentTile.down!.type == '.') currentTile = currentTile.down;
                                else len = 0;
                                break;
                        }
                        if (Program.VisualizationEnabled) PrintMap(currentTile, direction);
                    }
                }
                else
                {
                    if (move == "R")
                    {
                        switch (direction)
                        {
                            case 'R': direction = 'D'; break;
                            case 'L': direction = 'U'; break;
                            case 'U': direction = 'R'; break;
                            case 'D': direction = 'L'; break;
                        }
                    }
                    else if (move == "L")
                    {
                        switch (direction)
                        {
                            case 'R': direction = 'U'; break;
                            case 'L': direction = 'D'; break;
                            case 'U': direction = 'L'; break;
                            case 'D': direction = 'R'; break;
                        }
                    }
                }
            }

            int result = currentTile.pos.y * 1000 + currentTile.pos.x * 4;
            switch (direction)
            {
                case 'R': break;
                case 'L': result += 2; break;
                case 'U': result += 3; break;
                case 'D': result += 1; break;
            }

            return result.ToString();
        }

        Dictionary<(int x, int y), char> prevPos = new();
        Dictionary<char, char> dirTrans = new();
        Dictionary<char, string> decs = new();
        void PrintMap(Tile currentPos, char direction)
        {
            if (dirTrans.Count == 0)
            {
                dirTrans.Add('R', '>');
                dirTrans.Add('L', '<');
                dirTrans.Add('U', '^');
                dirTrans.Add('D', 'v');
                decs.Add('@', "\u001b[48;5;9m");
                decs.Add('>', "\u001b[38;5;9m");
                decs.Add('<', "\u001b[38;5;9m");
                decs.Add('^', "\u001b[38;5;9m");
                decs.Add('v', "\u001b[38;5;9m");
            }

            Dictionary<(int x, int y), char> dict = new();
            foreach (Tile tile in Tile.map.Values)
            {
                if (currentPos == tile)
                {
                    dict.Add(tile.pos, '@');
                    if(!prevPos.ContainsKey(tile.pos)) prevPos.Add(tile.pos, dirTrans[direction]);
                    else prevPos[tile.pos] = dirTrans[direction];
                }
                else if (prevPos.ContainsKey(tile.pos)) dict.Add(tile.pos, prevPos[tile.pos]);
                else dict.Add(tile.pos, tile.type);
            }
            Program.PrintData(Program.CreateStringFromDict(dict, decs), 0, true, true);
        }

        public override string Part2()
        {
            Tile.SetupCubeLinks();

            Tile currentTile = Tile.map[startPos];
            char direction = 'R';

            foreach (string move in moves)
            {
                int len = 0;
                if (int.TryParse(move, out len))
                {
                    while (len-- > 0)
                    {
                        switch (direction)
                        {
                            case 'R':
                                if (currentTile.right!.type == '.')
                                {
                                    direction = currentTile.newDirRight ?? direction;
                                    currentTile = currentTile.right;
                                }
                                else len = 0;
                                break;
                            case 'L':
                                if (currentTile.left!.type == '.')
                                {
                                    direction = currentTile.newDirLeft ?? direction;
                                    currentTile = currentTile.left;
                                }
                                else len = 0;
                                break;
                            case 'U':
                                if (currentTile.up!.type == '.')
                                {
                                    direction = currentTile.newDirUp ?? direction;
                                    currentTile = currentTile.up;
                                }
                                else len = 0;
                                break;
                            case 'D':
                                if (currentTile.down!.type == '.')
                                {

                                    direction = currentTile.newDirDown ?? direction;
                                    currentTile = currentTile.down;
                                }
                                else len = 0;
                                break;
                        }
                    }
                }
                else
                {
                    if (move == "R")
                    {
                        switch (direction)
                        {
                            case 'R': direction = 'D'; break;
                            case 'L': direction = 'U'; break;
                            case 'U': direction = 'R'; break;
                            case 'D': direction = 'L'; break;
                        }
                    }
                    else if (move == "L")
                    {
                        switch (direction)
                        {
                            case 'R': direction = 'U'; break;
                            case 'L': direction = 'D'; break;
                            case 'U': direction = 'L'; break;
                            case 'D': direction = 'R'; break;
                        }
                    }
                }
            }

            int result = currentTile.pos.y * 1000 + currentTile.pos.x * 4;
            switch (direction)
            {
                case 'R': break;
                case 'L': result += 2; break;
                case 'U': result += 3; break;
                case 'D': result += 1; break;
            }

            return result.ToString();
        }
    }
}
