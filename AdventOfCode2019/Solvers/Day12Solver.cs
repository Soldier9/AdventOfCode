using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2019.Solvers
{
    class Day12Solver : AbstractSolver
    {
        class Moon
        {
            public int X;
            public int Y;
            public int Z;
            public int VelX;
            public int VelY;
            public int VelZ;

            public Moon(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
                VelX = 0;
                VelY = 0;
                VelZ = 0;
            }


            public override int GetHashCode()
            {
                var hashCode = -1519173815;
                hashCode = hashCode * -1521134295 + X.GetHashCode();
                hashCode = hashCode * -1521134295 + Y.GetHashCode();
                hashCode = hashCode * -1521134295 + Z.GetHashCode();
                hashCode = hashCode * -1521134295 + VelX.GetHashCode();
                hashCode = hashCode * -1521134295 + VelY.GetHashCode();
                hashCode = hashCode * -1521134295 + VelZ.GetHashCode();
                return hashCode;
            }

            public int GetTotalEnergy()
            {
                int pot = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
                int kin = Math.Abs(VelX) + Math.Abs(VelY) + Math.Abs(VelZ);
                return pot * kin;
            }
        }

        public override string Part1()
        {
            List<Moon> moons = new List<Moon>();

            using (var input = File.OpenText(InputFile))
            {
                while(!input.EndOfStream)
                {
                    MatchCollection matches = Regex.Matches(input.ReadLine(), @"\-?\d+");

                    int x = int.Parse(matches[0].Value);
                    int y = int.Parse(matches[1].Value);
                    int z = int.Parse(matches[2].Value);

                    moons.Add(new Moon(x, y, z));
                }
            }

            for(int steps = 0; steps < 1000; steps++)
            {
                foreach(Moon moon in moons)
                {
                    foreach(Moon otherMoon in moons)
                    {
                        if (moon == otherMoon) continue;

                        if (moon.X < otherMoon.X) moon.VelX++;
                        else if (moon.X > otherMoon.X) moon.VelX--;

                        if (moon.Y < otherMoon.Y) moon.VelY++;
                        else if (moon.Y > otherMoon.Y) moon.VelY--;

                        if (moon.Z < otherMoon.Z) moon.VelZ++;
                        else if (moon.Z > otherMoon.Z) moon.VelZ--;
                    }
                }
                foreach (Moon moon in moons)
                {
                    moon.X += moon.VelX;
                    moon.Y += moon.VelY;
                    moon.Z += moon.VelZ;
                }
            }

            int totalSystemEnergy = 0;
            foreach(Moon moon in moons)
            {
                totalSystemEnergy += moon.GetTotalEnergy();
            }

            return totalSystemEnergy.ToString();
        }

        long GCD(long a, long b)
        {
            while(b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        long LCM(List<long> numbers)
        {
            long result = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                result = result * numbers[i] / GCD(result, numbers[i]);
            }
            return result;
        }

        public override string Part2()
        {
            List<Moon> moons = new List<Moon>();
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    MatchCollection matches = Regex.Matches(input.ReadLine(), @"\-?\d+");

                    int x = int.Parse(matches[0].Value);
                    int y = int.Parse(matches[1].Value);
                    int z = int.Parse(matches[2].Value);

                    moons.Add(new Moon(x, y, z));
                }
            }

            
            long steps = 0;

            long repeatingX = 0;
            long repeatingY = 0;
            long repeatingZ = 0;

            while (true)
            {
                foreach (Moon moon in moons)
                {
                    foreach (Moon otherMoon in moons)
                    {
                        if (moon == otherMoon) continue;

                        if (moon.X < otherMoon.X) moon.VelX++;
                        else if (moon.X > otherMoon.X) moon.VelX--;

                        if (moon.Y < otherMoon.Y) moon.VelY++;
                        else if (moon.Y > otherMoon.Y) moon.VelY--;

                        if (moon.Z < otherMoon.Z) moon.VelZ++;
                        else if (moon.Z > otherMoon.Z) moon.VelZ--;
                    }
                }
                foreach (Moon moon in moons)
                {
                    moon.X += moon.VelX;
                    moon.Y += moon.VelY;
                    moon.Z += moon.VelZ;
                }
                steps++;

                if (!moons.Exists(m => m.VelX != 0) && repeatingX == 0) repeatingX = steps;
                if (!moons.Exists(m => m.VelY != 0) && repeatingY == 0) repeatingY = steps;
                if (!moons.Exists(m => m.VelZ != 0) && repeatingZ == 0) repeatingZ = steps;

                if (repeatingX != 0 && repeatingY != 0 && repeatingZ != 0) break;
            }

            //return LCM(new List<long> { repeatingX, repeatingY, repeatingZ }).ToString();
            return (repeatingX * repeatingY * repeatingZ).ToString();
        }
    }
}