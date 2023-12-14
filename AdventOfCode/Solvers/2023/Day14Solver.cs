using System.Text;

namespace AdventOfCode.Solvers.Year2023
{
    class Day14Solver : AbstractSolver
    {
        Dictionary<(int x, int y), char> originalPlatform = new();
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
                        if (line[x] == '.') continue;
                        originalPlatform.Add((x, y), line[x]);
                    }
                    y++;
                }
            }

            height = originalPlatform.Max(i => i.Key.y) + 1;
            width = originalPlatform.Max(i => i.Key.x) + 1;

            Dictionary<(int x, int y), char> platform = TiltPlatform(originalPlatform, 'n');
            long result = 0;
            foreach (var roundRock in platform.Where(p => p.Value == 'O'))
            {
                result += height - roundRock.Key.y;
            }
            return result.ToString();
        }

        public override string Part2()
        {
            Dictionary<(int x, int y), char> platform = originalPlatform;
            Dictionary<string, int> previousPlatforms = new();
            bool foundCycle = false;
            for (int i = 0; i < 1000000000; i++)
            {
                platform = TiltPlatform(platform, 'n');
                platform = TiltPlatform(platform, 'w');
                platform = TiltPlatform(platform, 's');
                platform = TiltPlatform(platform, 'e');

                if (!foundCycle)
                {
                    string platformString = GetPlatformString(platform);
                    if (previousPlatforms.ContainsKey(platformString))
                    {
                        int cycleLength = i - previousPlatforms[platformString];
                        int cyclesLeft = (1000000000 - 1 - i) / cycleLength;
                        i += (cyclesLeft * cycleLength);
                        foundCycle = true;
                    }
                    else previousPlatforms.Add(platformString, i);
                }
            }

            long result = 0;
            foreach (var roundRock in platform.Where(p => p.Value == 'O'))
            {
                result += height - roundRock.Key.y;
            }
            return result.ToString();
        }

        public string GetPlatformString(Dictionary<(int x, int y), char> platform)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append(platform.ContainsKey((x, y)) ? '1' : '0');
                }
            }
            return sb.ToString();
        }

        public Dictionary<(int x, int y), char> TiltPlatform(Dictionary<(int x, int y), char> platform, char direction)
        {
            Dictionary<(int x, int y), char> tiltedPlatform = platform;
            bool somethingMoved;
            do
            {
                somethingMoved = false;
                Dictionary<(int x, int y), char> nextPlatform = new();
                foreach (var item in tiltedPlatform)
                {
                    (int x, int y) nextPos = (-1, -1);
                    switch (direction)
                    {
                        case 'n': nextPos = (item.Key.x, item.Key.y - 1); break;
                        case 's': nextPos = (item.Key.x, item.Key.y + 1); break;
                        case 'w': nextPos = (item.Key.x - 1, item.Key.y); break;
                        case 'e': nextPos = (item.Key.x + 1, item.Key.y); break;
                    };

                    if (item.Value == '#' || nextPos.x < 0 || nextPos.x >= width || nextPos.y < 0 || nextPos.y >= height) nextPlatform.Add(item.Key, item.Value);
                    else if (!tiltedPlatform.ContainsKey(nextPos))
                    {
                        nextPlatform.Add(nextPos, item.Value);
                        somethingMoved = true;
                    }
                    else nextPlatform.Add(item.Key, item.Value);
                }
                tiltedPlatform = nextPlatform;
            } while (somethingMoved);

            return tiltedPlatform;
        }
    }
}
