namespace AdventOfCode.Solvers._2025;

internal class Day7Solver : AbstractSolver
{
    private readonly Dictionary<int, HashSet<int>> _splitters = new();
    private int _bottom;
    private int _beamStartColumn;

    public override string Part1()
    {
        var result = 0;
        var beams = new HashSet<int>();

        using (var input = File.OpenText(InputFile))
        {
            var y = 0;
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '^':
                            if (!_splitters.ContainsKey(x)) _splitters.Add(x, []);
                            _splitters[x].Add(y);
                            _bottom = y;
                            break;

                        case 'S':
                            _beamStartColumn = x;
                            beams.Add(x);
                            break;
                    }
                }

                y++;
            }
        }

        var currentLine = 0;
        while (currentLine <= _bottom)
        {
            var splitBeams = new HashSet<int>();
            foreach (var x in beams)
            {
                if (_splitters.ContainsKey(x) && _splitters[x].Contains(currentLine))
                {
                    splitBeams.Add(x - 1);
                    splitBeams.Add(x + 1);
                    result++;
                }
                else
                {
                    splitBeams.Add(x);
                }
            }

            beams = splitBeams;
            currentLine++;
        }

        return result.ToString();
    }

    private static readonly Dictionary<(int x, int y), long> Cache = new();
    private long CountTimeLines((int x, int y) beam)
    {
        if (Cache.TryGetValue(beam, out var result)) return result;
        if (_splitters.TryGetValue(beam.x, out var splittersInColumn))
        {
            try
            {
                var line = splittersInColumn.Where(y => y >= beam.y).Min();
                result += CountTimeLines((beam.x - 1, line));
                result += CountTimeLines((beam.x + 1, line));

                Cache.Add(beam, result);
                return result;
            }
            catch (InvalidOperationException)
            {
            }
        }

        result++;
        Cache.Add(beam, result);
        return result;
    }

    public override string Part2()
    {
       var result = CountTimeLines((_beamStartColumn, 0));
        return result.ToString();
    }
}