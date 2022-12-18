namespace AdventOfCode.Solvers.Year2022
{
    class Day18Solver : AbstractSolver
    {
        HashSet<(int x, int y, int z)> drops = new();
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] coords = input.ReadLine()!.Split(",");
                    _ = drops.Add((int.Parse(coords[0]!), int.Parse(coords[1]!), int.Parse(coords[2]!)));
                }
            }

            int result = 0;
            foreach ((int x, int y, int z) drop in drops)
            {
                int subtract = 0;
                for (int x = -1; x < 2; x += 2) if (drops.Contains((drop.x + x, drop.y, drop.z))) subtract++;
                for (int y = -1; y < 2; y += 2) if (drops.Contains((drop.x, drop.y + y, drop.z))) subtract++;
                for (int z = -1; z < 2; z += 2) if (drops.Contains((drop.x, drop.y, drop.z + z))) subtract++;

                result += (6 - subtract);
            }

            return result.ToString();
        }

        HashSet<(int x, int y, int z)> CalcOutside()
        {
            HashSet<(int x, int y, int z)> outside = new();
            Queue<(int x, int y, int z)> queue = new();

            (int x, int y, int z) placeOutside = (drops.MinBy(d => d.x).x - 1, drops.MinBy(d => d.y).y - 1, drops.MinBy(d => d.z).z - 1);
            _ = outside.Add(placeOutside);
            queue.Enqueue(placeOutside);

            (int x, int y, int z) minBounds = (drops.MinBy(d => d.x).x - 2, drops.MinBy(d => d.y).y - 2, drops.MinBy(d => d.z).z - 2);
            (int x, int y, int z) maxBounds = (drops.MaxBy(d => d.x).x + 2, drops.MaxBy(d => d.y).y + 2, drops.MaxBy(d => d.z).z + 2);

            while (queue.Count > 0) CheckPosition(outside, queue, minBounds, maxBounds);
            return outside;
        }

        void CheckPosition(HashSet<(int x, int y, int z)> outside, Queue<(int x, int y, int z)> queue, (int x, int y, int z) minBounds, (int x, int y, int z) maxBounds)
        {
            if (queue.Count == 0) return;
            (int x, int y, int z) pos = queue.Dequeue();

            HashSet<(int x, int y, int z)> neighbors = new();
            for (int x = -1; x < 2; x += 2) if (pos.x + x > minBounds.x && pos.x + x < maxBounds.x) _ = neighbors.Add((pos.x + x, pos.y, pos.z));
            for (int y = -1; y < 2; y += 2) if (pos.y + y > minBounds.y && pos.y + y < maxBounds.y) _ = neighbors.Add((pos.x, pos.y + y, pos.z));
            for (int z = -1; z < 2; z += 2) if (pos.z + z > minBounds.z && pos.z + z < maxBounds.z) _ = neighbors.Add((pos.x, pos.y, pos.z + z));

            foreach ((int x, int y, int z) neighbor in neighbors)
            {
                if (!drops.Contains(neighbor) && !outside.Contains(neighbor))
                {
                    outside.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        public override string Part2()
        {
            HashSet<(int x, int y, int z)> outside = CalcOutside();

            int result = 0;
            foreach ((int x, int y, int z) drop in drops)
            {
                int subtract = 0;
                for (int x = -1; x < 2; x += 2) if (drops.Contains((drop.x + x, drop.y, drop.z)) || !outside.Contains((drop.x + x, drop.y, drop.z))) subtract++;
                for (int y = -1; y < 2; y += 2) if (drops.Contains((drop.x, drop.y + y, drop.z)) || !outside.Contains((drop.x, drop.y + y, drop.z))) subtract++;
                for (int z = -1; z < 2; z += 2) if (drops.Contains((drop.x, drop.y, drop.z + z)) || !outside.Contains((drop.x, drop.y, drop.z + z))) subtract++;

                result += (6 - subtract);
            }

            return result.ToString();
        }
    }
}
