using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2019
{
    class Day10Solver : AbstractSolver
    {
        class Line : IComparable
        {
            public readonly double A;
            public readonly double B;

            public Line(double a, double b)
            {
                A = a;
                B = b;
            }

            public override bool Equals(object obj)
            {
                Line other = (Line)obj;
                return (other.A == A && other.B == B);
            }

            public int CompareTo(object obj)
            {
                Line other = (Line)obj;
                return A.CompareTo(other.A);
            }

            public override int GetHashCode()
            {
                var hashCode = -1817952719;
                hashCode = hashCode * -1521134295 + A.GetHashCode();
                hashCode = hashCode * -1521134295 + B.GetHashCode();
                return hashCode;
            }

            public override string ToString()
            {
                return A.ToString() + "X + " + B.ToString();
            }
        }

        class Distances
        {
            public SortedDictionary<double, Asteroid> OneWay = new SortedDictionary<double, Asteroid>();
            public SortedDictionary<double, Asteroid> TheOtherWay = new SortedDictionary<double, Asteroid>();
        }

        class Asteroid
        {
            public readonly int X;
            public readonly int Y;

            public Dictionary<Line, Distances> OtherAsteroids = new Dictionary<Line, Distances>();

            public Asteroid(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int FindOtherVisibleAsteroids(HashSet<Asteroid> allAsteroids)
            {
                foreach (Asteroid otherAsteroid in allAsteroids)
                {
                    if (otherAsteroid == this) continue;

                    Line line;
                    if (Y == otherAsteroid.Y)
                    {
                        line = new Line(0, Y);
                    }
                    else if (X == otherAsteroid.X)
                    {
                        line = new Line(double.NegativeInfinity, 0); // NegativeInfinity causes it to be first in a sort
                    }
                    else
                    {
                        double a = (Y - otherAsteroid.Y) / (double)(X - otherAsteroid.X);
                        double b = Y - (a * X);
                        line = new Line(a, b);
                    }
                    double distance = Math.Sqrt(Math.Pow(X - otherAsteroid.X, 2) + Math.Pow(Y - otherAsteroid.Y, 2));

                    if (!OtherAsteroids.ContainsKey(line))
                    {
                        Distances distances = new Distances();
                        if (otherAsteroid.X > X || (otherAsteroid.X == X && otherAsteroid.Y < Y))
                        {
                            distances.OneWay.Add(distance, otherAsteroid);
                        }
                        else
                        {
                            distances.TheOtherWay.Add(distance, otherAsteroid);
                        }
                        OtherAsteroids.Add(line, distances);
                    }
                    else
                    {
                        if (otherAsteroid.X > X || (otherAsteroid.X == X && otherAsteroid.Y < Y))
                        {
                            OtherAsteroids[line].OneWay.Add(distance, otherAsteroid);
                        }
                        else
                        {
                            OtherAsteroids[line].TheOtherWay.Add(distance, otherAsteroid);
                        }
                    }
                }

                int otherVisibleAsteroids = 0;
                foreach (Distances distances in OtherAsteroids.Values)
                {
                    if (distances.OneWay.Count > 0) otherVisibleAsteroids++;
                    if (distances.TheOtherWay.Count > 0) otherVisibleAsteroids++;
                }

                return otherVisibleAsteroids;
            }
        }

        HashSet<Asteroid> Asteroids = new HashSet<Asteroid>();
        Asteroid BestAsteroid;
        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                int y = 0;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    for (int x = 0; x < line.Length; x++)
                    {
                        if (line[x] == '#') Asteroids.Add(new Asteroid(x, y));
                    }
                    y++;
                }
            }

            int mostVisibleOtherAsteroids = 0;
            foreach (Asteroid asteroid in Asteroids)
            {
                int visibleOtherAsteroids = asteroid.FindOtherVisibleAsteroids(Asteroids);
                if (mostVisibleOtherAsteroids < visibleOtherAsteroids)
                {
                    mostVisibleOtherAsteroids = visibleOtherAsteroids;
                    BestAsteroid = asteroid;
                }
            }

            return mostVisibleOtherAsteroids.ToString();
        }

        public override string Part2()
        {
            List<Asteroid> vaporizations = new List<Asteroid>();

            bool oneWay = true;
            while (Asteroids.Count - 1 > vaporizations.Count)
            {
                foreach (KeyValuePair<Line, Distances> otherAsteroid in BestAsteroid.OtherAsteroids.OrderBy(a => a.Key))
                {
                    if (oneWay && otherAsteroid.Value.OneWay.Count > 0)
                    {
                        KeyValuePair<double, Asteroid> nextToVaporize = otherAsteroid.Value.OneWay.First();
                        vaporizations.Add(nextToVaporize.Value);
                        otherAsteroid.Value.OneWay.Remove(nextToVaporize.Key);
                    }
                    else if (!oneWay && otherAsteroid.Value.TheOtherWay.Count > 0)
                    {
                        KeyValuePair<double, Asteroid> nextToVaporize = otherAsteroid.Value.TheOtherWay.First();
                        vaporizations.Add(nextToVaporize.Value);
                        otherAsteroid.Value.TheOtherWay.Remove(nextToVaporize.Key);
                    }
                }
                oneWay = !oneWay;
            }

            return ((vaporizations[199].X * 100) + vaporizations[200].Y).ToString();
        }
    }
}
