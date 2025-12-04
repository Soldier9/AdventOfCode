namespace AdventOfCode.Solvers._2025;

internal class Day4Solver : AbstractSolver
{
    HashSet<(int x, int y)> _paperLocations = new ();
    
    public override string Part1()
    {
        var result = 0;

        using (var input = File.OpenText(InputFile))
        {
            int y = 0;
            while (!input.EndOfStream)
            {
                var line = input.ReadLine()!;
                for (var x = 0; x < line.Length; x++) if (line[x] == '@') _paperLocations.Add((x, y));
                y++;
            }
        }
        
        foreach(var pos in _paperLocations) if(GetPaperNeighbors(pos).Count < 4) result++;

        return result.ToString();
    }

    HashSet<(int x, int y)> GetPaperNeighbors((int x, int y) pos)
    {
        HashSet<(int x, int y)> result = new();
        for (var xOffset = -1; xOffset <= 1; xOffset++)
        for (var yOffset = -1; yOffset <= 1; yOffset++)
        {
            if (xOffset == 0 && yOffset == 0) continue;
            if(_paperLocations.Contains((pos.x + xOffset, pos.y + yOffset)))result.Add((pos.x + xOffset, pos.y + yOffset));
        }
        return result;
    }

    public override string Part2()
    {
        var result = 0;
        while (_paperLocations.Count > 0)
        {
            var updatedPaperLocations = new HashSet<(int x, int y)>();
            foreach (var pos in _paperLocations)
            {
                if (GetPaperNeighbors(pos).Count >= 4) updatedPaperLocations.Add(pos);
                else result++;
            }
            if(_paperLocations.Count == updatedPaperLocations.Count) break;
            _paperLocations = updatedPaperLocations;
        }

        return result.ToString();
    }
}