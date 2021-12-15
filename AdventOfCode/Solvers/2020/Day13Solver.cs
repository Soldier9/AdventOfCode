namespace AdventOfCode.Solvers.Year2020
{
    class Day13Solver : AbstractSolver
    {
        public override string Part1()
        {
            int earliestDeparture;
            Dictionary<int, int> busses = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                earliestDeparture = int.Parse(input.ReadLine()!);
                string[] ids = input.ReadLine()!.Split(',');
                foreach (string id in ids)
                {
                    if (id != "x") busses.Add(int.Parse(id), 0);
                }
            }

            foreach (int bus in busses.Keys.ToArray())
            {
                busses[bus] = bus - (earliestDeparture % bus);
            }

            KeyValuePair<int, int> earliestBus = busses.OrderBy(b => b.Value).First();
            return (earliestBus.Value * earliestBus.Key).ToString();
        }

        public override string Part2()
        {
            Dictionary<int, int> busses = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                _ = input.ReadLine();
                string[] ids = input.ReadLine()!.Split(',');
                int offset = 0;
                foreach (string id in ids)
                {
                    if (id != "x") busses.Add(int.Parse(id), offset);
                    offset++;
                }
            }
            busses = busses.OrderBy(b => b.Key).ToDictionary(b => b.Key, b => b.Value);

            long departureTime = -1;
            long advanceBy = 1;
            bool allTimesMatch = false;
            while (!allTimesMatch)
            {
                departureTime += advanceBy;
                allTimesMatch = true;

                while (allTimesMatch && busses.Count > 0)
                {
                    KeyValuePair<int, int> bus = busses.First();
                    if ((departureTime + bus.Value) % bus.Key == 0)
                    {
                        advanceBy *= bus.Key;
                        _ = busses.Remove(bus.Key);
                    }
                    else
                    {
                        allTimesMatch = false;
                        break;
                    }
                }
            }

            return departureTime.ToString();
        }
    }
}
