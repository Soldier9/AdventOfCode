using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day4Solver : AbstractSolver
    {

        class Board : List<List<int>>
        {
            private readonly int GridSize;
            private readonly List<List<bool>> Called = new List<List<bool>>();
            private bool Done = false;

            public Board(int gridSize = 5)
            {
                GridSize = gridSize;
                for (int i = 0; i < GridSize; i++) Called.Add(new List<bool>(new bool[] { false, false, false, false, false }));
            }

            public bool HasWon()
            {
                if (Done) return Done;

                for (int x = 0; x < GridSize; x++)
                    for (int y = 0; y < GridSize; y++)
                        if (!Called[x][y]) break;
                        else if (y == GridSize - 1) return Done = true;


                for (int x = 0; x < GridSize; x++)
                    for (int y = 0; y < GridSize; y++)
                        if (!Called[y][x]) break;
                        else if (y == GridSize - 1) return Done = true;

                return false;
            }

            public void NumberCalled(int num)
            {
                for (int x = 0; x < GridSize; x++)
                    for (int y = 0; y < GridSize; y++)
                        if (this[x][y] == num)
                        {
                            Called[x][y] = true;
                            return;
                        }
            }

            public int WinningScore(int num)
            {
                int result = 0;

                for (int x = 0; x < GridSize; x++)
                    for (int y = 0; y < GridSize; y++)
                        if (!Called[x][y]) result += this[x][y];

                return result * num;
            }

            public void ResetBoard()
            {
                Done = false;
                Called.Clear();
                for (int i = 0; i < GridSize; i++) Called.Add(new List<bool>(new bool[] { false, false, false, false, false }));
            }
        }

        List<int> Numbers = new List<int>();
        List<Board> Boards = new List<Board>();

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                Numbers = new List<int>(input.ReadLine().Split(',').Select(n => int.Parse(n)));

                Board nextBoard = null;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line.Length == 0)
                    {
                        if (nextBoard != null) Boards.Add(nextBoard);
                        nextBoard = new Board();
                        continue;
                    }

                    nextBoard.Add(new List<int>(line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n))));
                }
                Boards.Add(nextBoard);
            }

            foreach (int num in Numbers)
            {
                foreach (Board board in Boards)
                {
                    board.NumberCalled(num);
                    if (board.HasWon())
                    {
                        return board.WinningScore(num).ToString();
                    }
                }
            }

            return "No winning board found".ToString();
        }

        public override string Part2()
        {
            foreach (Board board in Boards) board.ResetBoard();

            foreach (int num in Numbers)
            {
                foreach (Board board in Boards)
                {
                    board.NumberCalled(num);
                    if (board.HasWon() && Boards.Count == 1) return board.WinningScore(num).ToString();
                }
                Boards.RemoveAll(b => b.HasWon());
            }

            return "Not all boards will win?!".ToString();
        }
    }
}
