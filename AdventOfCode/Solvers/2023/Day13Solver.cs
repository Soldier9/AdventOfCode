using System.Text;

namespace AdventOfCode.Solvers.Year2023
{

    class Day13Solver : AbstractSolver
    {
        List<Dictionary<(int x, int y), char>> patterns = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Dictionary<(int x, int y), char> pattern = new();
                int y = -1;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.Length == 0)
                    {
                        patterns.Add(pattern);
                        pattern = new();
                        y = -1;
                        continue;
                    }
                    y++;
                    for (int x = 0; x < line.Length; x++) pattern.Add((x, y), line[x]);
                }
                patterns.Add(pattern);
            }

            long result = 0;
            foreach (Dictionary<(int x, int y), char> pattern in patterns)
            {
                int height = pattern.Max(p => p.Key.y);
                int width = pattern.Max(p => p.Key.x);

                for (int x = 0; x < width; x++)
                {
                    int col1 = x;
                    int col2 = x + 1;
                    bool isMirror = true;
                    while (col1 >= 0 && col2 <= width)
                    {
                        if (GetColumn(pattern, col1) != GetColumn(pattern, col2))
                        {
                            isMirror = false;
                            break;
                        }
                        col1--;
                        col2++;
                    }
                    if (isMirror)
                    {
                        result += (x + 1);
                        break;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    int row1 = y;
                    int row2 = y + 1;
                    bool isMirror = true;
                    while (row1 >= 0 && row2 <= height)
                    {
                        if (GetRow(pattern, row1) != GetRow(pattern, row2))
                        {
                            isMirror = false;
                            break;
                        }
                        row1--;
                        row2++;
                    }
                    if (isMirror)
                    {
                        result += (100 * (y + 1));
                        break;
                    }
                }
            }

            return result.ToString();
        }

        public static string GetColumn(Dictionary<(int x, int y), char> pattern, int x)
        {
            StringBuilder sb = new();
            for (int y = 0; y <= pattern.Max(p => p.Key.y); y++) sb.Append(pattern[(x, y)]);
            return sb.ToString();
        }
        public static string GetRow(Dictionary<(int x, int y), char> pattern, int y)
        {
            StringBuilder sb = new();
            for (int x = 0; x <= pattern.Max(p => p.Key.x); x++) sb.Append(pattern[(x, y)]);
            return sb.ToString();
        }

        public static int Differences(string val1, string val2)
        {
            int diffs = 0;
            for(int i = 0 ; i < val1.Length; i++) if (val1[i] != val2[i]) diffs++;
            return diffs;
        }


        public override string Part2()
        {
            long result = 0;
            foreach (Dictionary<(int x, int y), char> pattern in patterns)
            {
                int height = pattern.Max(p => p.Key.y);
                int width = pattern.Max(p => p.Key.x);

                for (int x = 0; x < width; x++)
                {
                    int col1 = x;
                    int col2 = x + 1;
                    int diffs = 0;
                    while (col1 >= 0 && col2 <= width)
                    {
                        diffs += Differences(GetColumn(pattern, col1), GetColumn(pattern, col2));
                        if(diffs > 1) break;
                        col1--;
                        col2++;
                    }
                    if (diffs == 1)
                    {
                        result += (x + 1);
                        break;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    int row1 = y;
                    int row2 = y + 1;
                    int diffs = 0;
                    while (row1 >= 0 && row2 <= height)
                    {
                        diffs += Differences(GetRow(pattern, row1), GetRow(pattern, row2));
                        if (diffs > 1) break;
                        row1--;
                        row2++;
                    }
                    if (diffs == 1)
                    {
                        result += (100 * (y + 1));
                        break;
                    }
                }
            }

            return result.ToString();
        }
    }
}