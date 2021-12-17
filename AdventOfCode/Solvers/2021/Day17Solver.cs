using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2021
{
    class Day17Solver : AbstractSolver
    {
        (int x, int y) MinTarget = new();
        (int x, int y) MaxTarget = new();

        bool TestTrajectory((int x, int y) velocity, out int maxY)
        {
            (int x, int y) location = (0, 0);
            maxY = location.y;

            while (true)
            {
                location.x += velocity.x;
                location.y += velocity.y;
                maxY = Math.Max(location.y, maxY);

                velocity.x += (velocity.x > 0 ? -1 : (velocity.x < 0 ? 1 : 0));
                velocity.y--;

                if (location.x > MaxTarget.x || location.y < MinTarget.y) return false;
                if (location.x >= MinTarget.x && location.x <= MaxTarget.x && location.y >= MinTarget.y && location.y <= MaxTarget.y) return true;
            }
        }

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Match extract = new Regex(@".*x=(-?\d+)\.\.(-?\d+).*y=(-?\d+)\.\.(-?\d+)").Match(input.ReadLine()!);
                    MinTarget = (int.Parse(extract.Groups[1].Value), int.Parse(extract.Groups[3].Value));
                    MaxTarget = (int.Parse(extract.Groups[2].Value), int.Parse(extract.Groups[4].Value));
                }
            }
            
            Dictionary<(int, int), int> validVelocities = new();
            for (int x = 0; x <= MaxTarget.x; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    (int x, int y) velocity = (x, y);
                    if (TestTrajectory(velocity, out int maxY))
                    {
                        validVelocities.Add(velocity, maxY);
                    }
                }
            }

            return validVelocities.MaxBy(v => v.Value).Value.ToString();
        }

        public override string Part2()
        {
            Dictionary<(int, int), int> validVelocities = new();
            for (int x = 0; x <= MaxTarget.x; x++)
            {
                for (int y = MinTarget.y; y < 1000; y++)
                {
                    (int x, int y) velocity = (x, y);
                    if (TestTrajectory(velocity, out int maxY))
                    {
                        validVelocities.Add(velocity, maxY);
                    }
                }
            }

            return validVelocities.Count.ToString();
        }
    }
}
