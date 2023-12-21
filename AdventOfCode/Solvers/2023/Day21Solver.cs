using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solvers.Year2023
{
    class Day21Solver : AbstractSolver
    {
        HashSet<(int x, int y)> Stones = new();
        (int x, int y) StartingPos;
        int height = 0;
        int width = 0;
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
                        if (line[x] == '#') Stones.Add((x, y));
                        else if (line[x] == 'S') StartingPos = (x, y);
                        width = Math.Max(width, x + 1);
                    }
                    y++;
                    height = Math.Max(height, y);
                }
            }

            HashSet<(int x, int y)> currentPossiblePositions = new();
            currentPossiblePositions.Add(StartingPos);
            for (int i = 0; i < 64; i++)
            {
                HashSet<(int x, int y)> nextPossiblePositions = new();
                foreach ((int x, int y) position in currentPossiblePositions)
                {
                    foreach ((int x, int y) move in GetPossibleMoves(position))
                    {
                        nextPossiblePositions.Add(move);
                    }
                }
                currentPossiblePositions = nextPossiblePositions;
            }

            return currentPossiblePositions.Count.ToString();
        }

        HashSet<(int x, int y)> GetPossibleMoves((int x, int y) fromPos)
        {
            HashSet<(int x, int y)> moves = new()
            {
                (fromPos.x + 1, fromPos.y),
                (fromPos.x - 1, fromPos.y),
                (fromPos.x, fromPos.y + 1),
                (fromPos.x, fromPos.y - 1)
            };
            moves.RemoveWhere(m => m.x < 0 || m.x >= width || m.y < 0 || m.y >= height);
            foreach ((int x, int y) move in moves) if (Stones.Contains(move)) moves.Remove(move);
            return moves;
        }

        public override string Part2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("interpolate{{");
            int dataPoints = 0;
            HashSet<(int x, int y)> currentPossiblePositions = new();
            currentPossiblePositions.Add(StartingPos);
            for (int i = 0; i < 26501365; i++)
            {
                HashSet<(int x, int y)> nextPossiblePositions = new();
                foreach ((int x, int y) position in currentPossiblePositions)
                {
                    foreach ((int x, int y) move in GetPossibleMovesPart2(position))
                    {
                        nextPossiblePositions.Add(move);
                    }
                }
                currentPossiblePositions = nextPossiblePositions;
                if (((i + 1) - 65) % width == 0)
                {
                    if (i > width) sb.Append(", {");
                    sb.Append((i + 1) + ", " + currentPossiblePositions.Count + "}");
                    if(++dataPoints == 3)
                    {
                        sb.Append("}");
                        break;
                    }
                }
            }

            return "Feed this into WolframAlpha and solve for 26501365: \r\n" + sb.ToString();
        }

        HashSet<(int x, int y)> GetPossibleMovesPart2((int x, int y) fromPos)
        {
            HashSet<(int x, int y)> moves = new()
            {
                (fromPos.x + 1, fromPos.y),
                (fromPos.x - 1, fromPos.y),
                (fromPos.x, fromPos.y + 1),
                (fromPos.x, fromPos.y - 1)
            };

            foreach ((int x, int y) move in moves)
            {
                (int x, int y) checkedPos = (move.x % width, move.y % height);
                if (checkedPos.x < 0) checkedPos.x += width;
                if (checkedPos.y < 0) checkedPos.y += height;
                if (Stones.Contains(checkedPos)) moves.Remove(move);
            }
            return moves;
        }
    }
}
