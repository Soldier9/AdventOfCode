using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2024
{
    class Day14Solver : AbstractSolver
    {
        class Robot
        {
            public (int x, int y) Position;
            (int x, int y) Velocity;

            static Regex inputParser = new(@"p\=(\d+),(\d+)\sv=(\-?\d+),(\-?\d+)");
            public Robot(string input)
            {
                Match parsed = inputParser.Match(input);
                Position = (int.Parse(parsed.Groups[1].Value), int.Parse(parsed.Groups[2].Value));
                Velocity = (int.Parse(parsed.Groups[3].Value), int.Parse(parsed.Groups[4].Value));
            }

            public void Walk()
            {
                Position = AddPos(Position, Velocity);

                if (Position.x < 0) Position.x += GridSize.x;
                if (Position.y < 0) Position.y += GridSize.y;
                if (Position.x >= GridSize.x) Position.x -= GridSize.x;
                if (Position.y >= GridSize.y) Position.y -= GridSize.y;
            }

            private (int x, int y) AddPos((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
        }

        static (int x, int y) GridSize = (101, 103);
        List<Robot> Robots = [];

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    Robots.Add(new Robot(line));
                }
            }

            for (int i = 0; i < 100; i++)
            {
                foreach (Robot robot in Robots) robot.Walk();
            }

            List<int> quadCount = [0, 0, 0, 0];
            foreach (Robot robot in Robots)
            {
                if (robot.Position.x < GridSize.x / 2 && robot.Position.y < GridSize.y / 2) quadCount[0]++;
                if (robot.Position.x > GridSize.x / 2 && robot.Position.y < GridSize.y / 2) quadCount[1]++;
                if (robot.Position.x < GridSize.x / 2 && robot.Position.y > GridSize.y / 2) quadCount[2]++;
                if (robot.Position.x > GridSize.x / 2 && robot.Position.y > GridSize.y / 2) quadCount[3]++;
            }

            int result = 1;
            quadCount.ForEach(q => result *= q);
            return result.ToString();
        }

        public override string Part2()
        {
            Robots.Clear();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    Robots.Add(new Robot(line));
                }
            }

            int result = int.MaxValue;
            for (int i = 1; i < int.MaxValue; i++)
            {
                Robots.ForEach(r => r.Walk());

                HashSet<(int x, int y)> grid = [];
                foreach (Robot robot in Robots)
                {
                    if (grid.Contains(robot.Position))
                    {
                        grid.Clear();
                        break;
                    }
                    grid.Add(robot.Position);
                }

                if (grid.Count > 0)
                {
                    result = Math.Min(result, i);
                    string output = "Seconds Elapsed: " + i.ToString() + " (press Enter to End, any other key to continue)\r\n" + Program.CreateStringFromDict(Program.CreateDictFromEnumerable(grid, '#'));
                    Program.PrintData(output, 0, true, false);
                    if (Console.ReadKey().Key == ConsoleKey.Enter) break;
                }
            }

            return result.ToString();
        }
    }
}
