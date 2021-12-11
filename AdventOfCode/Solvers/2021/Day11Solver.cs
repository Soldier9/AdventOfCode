using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2021
{
    class Day11Solver : AbstractSolver
    {
        class Octopus
        {
            static public Octopus[,] Grid = new Octopus[10, 10];
            public int X;
            public int Y;
            public int Energy;
            public bool HasFlashed;

            public Octopus(int x, int y, int energy)
            {
                X = x;
                Y = y;
                Energy = energy;
                Octopus.Grid[x, y] = this;
            }

            public List<Octopus> GetNeighbors()
            {
                List<Octopus> neighbors = new List<Octopus>();

                if (X > 0 && Y > 0) neighbors.Add(Octopus.Grid[X - 1, Y - 1]);
                if (Y > 0) neighbors.Add(Octopus.Grid[X, Y - 1]);
                if (Y > 0 && X < 9) neighbors.Add(Octopus.Grid[X + 1, Y - 1]);

                if (X > 0) neighbors.Add(Octopus.Grid[X - 1, Y]);
                if (X < 9) neighbors.Add(Octopus.Grid[X + 1, Y]);

                if (X > 0 && Y < 9) neighbors.Add(Octopus.Grid[X - 1, Y + 1]);
                if (Y < 9) neighbors.Add(Octopus.Grid[X, Y + 1]);
                if (X < 9 && Y < 9) neighbors.Add(Octopus.Grid[X + 1, Y + 1]);

                return neighbors;
            }

            public static List<Octopus> GetAllReadyToFlash()
            {
                List<Octopus> readyToFlash = new List<Octopus>();
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        if (Octopus.Grid[x, y].Energy > 9) readyToFlash.Add(Octopus.Grid[x, y]);
                    }
                }
                return readyToFlash;
            }
        }

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                int tmpX = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    int tmpY = 0;
                    foreach (char c in line)
                    {
                        new Octopus(tmpX, tmpY, int.Parse(c.ToString()));
                        tmpY++;
                    }
                    tmpX++;
                }
            }

            int flashes = 0;
            for (int n = 0; n < 100; n++)
            {
                foreach (Octopus octopus in Octopus.Grid)
                {
                    octopus.Energy++;
                    octopus.HasFlashed = false;
                }

                List<Octopus> readyToFlash = Octopus.GetAllReadyToFlash();
                while (readyToFlash.Count > 0)
                {
                    foreach (Octopus octopus in readyToFlash)
                    {
                        foreach (Octopus neighbor in octopus.GetNeighbors().Where(o => !o.HasFlashed)) neighbor.Energy++;
                        octopus.HasFlashed = true;
                        octopus.Energy = 0;
                        flashes++;
                    }
                    readyToFlash = Octopus.GetAllReadyToFlash();
                }
            }

            return flashes.ToString();
        }

        public override string Part2()
        {
            using (var input = File.OpenText(InputFile))
            {
                int tmpX = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    int tmpY = 0;
                    foreach (char c in line)
                    {
                        new Octopus(tmpX, tmpY, int.Parse(c.ToString()));
                        tmpY++;
                    }
                    tmpX++;
                }
            }


            for (int n = 0; n > -1; n++)
            {
                int flashes = 0;
                foreach (Octopus octopus in Octopus.Grid)
                {
                    octopus.Energy++;
                    octopus.HasFlashed = false;
                }

                List<Octopus> readyToFlash = Octopus.GetAllReadyToFlash();
                while (readyToFlash.Count > 0)
                {
                    foreach (Octopus octopus in readyToFlash)
                    {
                        foreach (Octopus neighbor in octopus.GetNeighbors().Where(o => !o.HasFlashed)) neighbor.Energy++;
                        octopus.HasFlashed = true;
                        octopus.Energy = 0;
                        flashes++;
                    }
                    readyToFlash = Octopus.GetAllReadyToFlash();
                }

                if (flashes == 100) return (n + 1).ToString();
            }

            return ":(".ToString();
        }
    }
}
