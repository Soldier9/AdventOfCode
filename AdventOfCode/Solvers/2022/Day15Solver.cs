using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day15Solver : AbstractSolver
    {
        Dictionary<(int x, int y), int> sensors = new();
        HashSet<(int x, int y)> beacons = new();

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parser = new(@"x=(-?\d+), y=(-?\d+):.+x=(-?\d+), y=(-?\d+)");
                while (!input.EndOfStream)
                {
                    Match match = parser.Match(input.ReadLine()!);
                    (int x, int y) sensor = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                    (int x, int y) beacon = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

                    int dist = manDist(sensor, beacon);
                    sensors.Add(sensor, dist);
                    _ = beacons.Add(beacon);
                }
            }

            int minX = sensors.MinBy(s => s.Key.x).Key.x - sensors.MaxBy(s => s.Value).Value;
            int maxX = sensors.MaxBy(s => s.Key.x).Key.x + sensors.MaxBy(s => s.Value).Value;
            int result = 0;

            for (int x = minX; x <= maxX; x++)
            {
                (int x, int y) testPos = (x, 2000000);
                if (beacons.Contains(testPos)) continue;
                bool validPos = false;
                foreach (KeyValuePair<(int x, int y), int> sensor in sensors)
                {
                    if (manDist(testPos, sensor.Key) <= sensor.Value)
                    {
                        validPos = true;
                        break;
                    }
                }
                if (validPos) result++;
            }

            return result.ToString();
        }

        public int manDist((int x, int y) sensor, (int x, int y) beacon) => Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);

        public override string Part2()
        {
            for (int x = 0; x <= 4000000; x++)
            {
                int yInc;
                for (int y = 0; y <= 4000000; y += yInc)
                {
                    (int x, int y) testPos = (x, y);
                    yInc = 1;
                    bool validPos = true;
                    foreach (KeyValuePair<(int x, int y), int> sensor in sensors)
                    {
                        int dist = manDist(testPos, sensor.Key);
                        if (dist <= sensor.Value)
                        {
                            yInc = Math.Max(sensor.Value - dist + 1, yInc);
                            validPos = false;
                            break;
                        }
                    }
                    if (validPos) return (((BigInteger)testPos.x) * 4000000 + testPos.y).ToString();
                }
            }

            return "No solution found".ToString();
        }
    }
}
