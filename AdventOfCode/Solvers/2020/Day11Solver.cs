namespace AdventOfCode.Solvers.Year2020
{
    class Day11Solver : AbstractSolver
    {
        readonly Dictionary<(int, int), char> seats = new();
        readonly List<(int, int)> directions = new();
        int rows, columns;
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    int x = -1;
                    foreach (char position in input.ReadLine()!)
                    {
                        x++;
                        if (position == '.') continue;
                        seats.Add((x, y), position);
                    }
                    columns = x + 1;
                    y++;
                }
                rows = y;
            }
            directions.Add((-1, -1));
            directions.Add((0, -1));
            directions.Add((1, -1));
            directions.Add((-1, 0));
            directions.Add((1, 0));
            directions.Add((-1, 1));
            directions.Add((0, 1));
            directions.Add((1, 1));

            bool somethingChanged = true;
            Dictionary<(int, int), char> currentSeats = new(seats);
            while (somethingChanged)
            {
                somethingChanged = false;
                Dictionary<(int, int), char> nextSeats = new();
                foreach ((int x, int y) in currentSeats.Keys)
                {
                    int adjacentOccupied = 0;
                    foreach ((int offsetX, int offsetY) in directions)
                    {
                        if (currentSeats.ContainsKey((x + offsetX, y + offsetY)) && currentSeats[(x + offsetX, y + offsetY)] == '#') adjacentOccupied++;
                    }

                    char state = currentSeats[(x, y)];
                    if (state == 'L' && adjacentOccupied == 0)
                    {
                        state = '#';
                        somethingChanged = true;
                    }
                    else if (state == '#' && adjacentOccupied > 3)
                    {
                        state = 'L';
                        somethingChanged = true;
                    }
                    nextSeats.Add((x, y), state);
                }
                currentSeats = nextSeats;
            }

            int result = 0;
            foreach (char state in currentSeats.Values)
            {
                if (state == '#') result++;
            }
            return result.ToString();
        }

        public override string Part2()
        {
            bool somethingChanged = true;
            Dictionary<(int, int), char> currentSeats = new(seats);

            while (somethingChanged)
            {
                somethingChanged = false;
                Dictionary<(int, int), char> nextSeats = new();
                foreach ((int x, int y) in currentSeats.Keys)
                {
                    int adjacentOccupied = 0;
                    foreach ((int offsetX, int offsetY) in directions)
                    {
                        for (int i = 1; true; i++)
                        {
                            int lookX = x + (offsetX * i);
                            int lookY = y + (offsetY * i);
                            if (currentSeats.ContainsKey((lookX, lookY)))
                            {
                                if (currentSeats[(lookX, lookY)] == '#') adjacentOccupied++;
                                break;
                            }
                            else if (lookX < 0 || lookX >= columns || lookY < 0 || lookY >= rows)
                            {
                                break;
                            }
                        }
                    }

                    char state = currentSeats[(x, y)];
                    if (state == 'L' && adjacentOccupied == 0)
                    {
                        state = '#';
                        somethingChanged = true;
                    }
                    else if (state == '#' && adjacentOccupied > 4)
                    {
                        state = 'L';
                        somethingChanged = true;

                    }
                    nextSeats.Add((x, y), state);
                }
                currentSeats = nextSeats;
            }

            int result = 0;
            foreach (char state in currentSeats.Values)
            {
                if (state == '#') result++;
            }
            return result.ToString();
        }
    }
}