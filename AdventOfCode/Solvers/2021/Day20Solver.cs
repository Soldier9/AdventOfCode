using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day20Solver : AbstractSolver
    {
        public override bool HasVisualization => true;
        public override bool HasExtendedVisualization => true;

        public override string Part1()
        {
            string algorithm;
            Dictionary<(int x, int y), char> image = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                algorithm = input.ReadLine()!;
                _ = input.ReadLine();
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach (char c in line)
                    {
                        image.Add((x, y), c == '#' ? '1' : '0');
                        x++;
                    }
                    y++;
                }
            }

            (int min, int max) xBounds = (image.Keys.MinBy(k => k.x).x, image.Keys.MaxBy(k => k.x).x);
            (int min, int max) yBounds = (image.Keys.MinBy(k => k.y).y, image.Keys.MaxBy(k => k.y).y);
            for (int n = 0; n < 2; n++)
            {
                if (Program.ExtendedVisualization) PrintImage(image);
                xBounds.min--; xBounds.max++;
                yBounds.min--; yBounds.max++;
                Dictionary<(int, int), char> nextImage = new();
                for (int x = xBounds.min; x <= xBounds.max; x++)
                {
                    for (int y = yBounds.min; y <= yBounds.max; y++)
                    {
                        nextImage.Add((x, y), algorithm[GetBinaryValue((x, y), image, n)] == '#' ? '1' : '0');
                    }
                }

                image = nextImage;
            }

            if (Program.VisualizationEnabled) PrintImage(image);
            return image.Where(p => p.Value == '1').Count().ToString();
        }

        private static int GetBinaryValue((int x, int y) pos, Dictionary<(int, int), char> image, int enhanementRun)
        {
            char newPixels = (enhanementRun % 2).ToString()[0];

            StringBuilder sb = new();
            _ = sb.Append(image.TryGetValue((pos.x - 1, pos.y - 1), out char pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x, pos.y - 1), out pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x + 1, pos.y - 1), out pixel) ? pixel : newPixels);

            _ = sb.Append(image.TryGetValue((pos.x - 1, pos.y), out pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x, pos.y), out pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x + 1, pos.y), out pixel) ? pixel : newPixels);

            _ = sb.Append(image.TryGetValue((pos.x - 1, pos.y + 1), out pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x, pos.y + 1), out pixel) ? pixel : newPixels);
            _ = sb.Append(image.TryGetValue((pos.x + 1, pos.y + 1), out pixel) ? pixel : newPixels);

            return Convert.ToInt32(sb.ToString(), 2);
        }

        public override string Part2()
        {
            string algorithm;
            Dictionary<(int x, int y), char> image = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                algorithm = input.ReadLine()!;
                _ = input.ReadLine();
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int x = 0;
                    foreach (char c in line)
                    {
                        image.Add((x, y), c == '#' ? '1' : '0');
                        x++;
                    }
                    y++;
                }
            }

            (int min, int max) xBounds = (image.Keys.MinBy(k => k.x).x, image.Keys.MaxBy(k => k.x).x);
            (int min, int max) yBounds = (image.Keys.MinBy(k => k.y).y, image.Keys.MaxBy(k => k.y).y);
            for (int n = 0; n < 50; n++)
            {
                if (Program.ExtendedVisualization) PrintImage(image);
                xBounds.min--; xBounds.max++;
                yBounds.min--; yBounds.max++;
                Dictionary<(int, int), char> nextImage = new();
                for (int x = xBounds.min; x <= xBounds.max; x++)
                {
                    for (int y = yBounds.min; y <= yBounds.max; y++)
                    {
                        nextImage.Add((x, y), algorithm[GetBinaryValue((x, y), image, n)] == '#' ? '1' : '0');
                    }
                }

                image = nextImage;
            }

            if (Program.VisualizationEnabled) PrintImage(image);
            return image.Where(p => p.Value == '1').Count().ToString();
        }

        private static void PrintImage(Dictionary<(int x, int y), char> image)
        {
            StringBuilder sb = new();
            (int min, int max) xBounds = (image.Keys.MinBy(k => k.x).x, image.Keys.MaxBy(k => k.x).x);
            (int min, int max) yBounds = (image.Keys.MinBy(k => k.y).y, image.Keys.MaxBy(k => k.y).y);
            for (int x = xBounds.min; x <= xBounds.max; x++)
            {
                for (int y = yBounds.min; y <= yBounds.max; y++)
                {
                    _ = sb.Append(image[(x, y)] == '1' ? '#' : ' ');
                }
                _ = sb.Append("\r\n");
            }
            
            Program.PrintData(sb.ToString(), 1);
        }
    }
}