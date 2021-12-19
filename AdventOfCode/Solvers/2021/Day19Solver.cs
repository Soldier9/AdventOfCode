using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2021
{
    class Day19Solver : AbstractSolver
    {
        class Vector
        {
            public int X;
            public int Y;
            public int Z;

            public Vector(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Vector Add(Vector other)
            {
                return new Vector(X + other.X, Y + other.Y, Z + other.Z);
            }

            public Vector Subtract(Vector other)
            {
                return new Vector(X - other.X, Y - other.Y, Z - other.Z);
            }

            public Vector Turn()
            {
                return new Vector(-Y, X, Z);
            }

            public Vector Rotate()
            {
                return new Vector(X, Z, -Y);
            }

            public int ManhattanDistance(Vector other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
            }

            public override string ToString()
            {
                return ("[" + X + "," + Y + "," + Z + "]").ToString();
            }

            public override bool Equals(object? other)
            {
                if (other == null || other is not Vector) return false;
                Vector v = (Vector)other;
                if (X == v.X && Y == v.Y && Z == v.Z) return true;
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
            }
        }

        class Scanner
        {
            public readonly int Id;
            public Dictionary<Vector, Dictionary<Vector, Vector>> Probes = new();
            public readonly List<Dictionary<Vector, Dictionary<Vector, Vector>>> Rotations = new();
            public Vector? Position;

            public Scanner(string id)
            {
                Id = int.Parse(id);
                if (Id == 0) Position = new Vector(0, 0, 0);
            }

            public void SetPosition(Vector position, int rotation)
            {
                Position = position;

                Dictionary<Vector, Dictionary<Vector, Vector>> newProbelist = new();
                foreach (Vector probe in Rotations[rotation].Keys)
                {
                    newProbelist.Add(probe.Add(Position), Rotations[rotation][probe]);
                }
                Probes = newProbelist;
                Rotations.Clear();
            }

            public void AddProbe(Vector probe, int rotation = -1)
            {
                Dictionary<Vector, Vector> distances = new();
                if (rotation == -1)
                {
                    foreach (KeyValuePair<Vector, Dictionary<Vector, Vector>> existingProbe in Probes)
                    {
                        distances.Add(probe.Subtract(existingProbe.Key), existingProbe.Key);
                        existingProbe.Value.Add(existingProbe.Key.Subtract(probe), probe);
                    }
                    Probes.Add(probe, distances);
                }
                else
                {
                    if (Rotations.Count <= rotation) Rotations.Add(new Dictionary<Vector, Dictionary<Vector, Vector>>());
                    foreach (KeyValuePair<Vector, Dictionary<Vector, Vector>> existingProbe in Rotations[rotation])
                    {
                        distances.Add(probe.Subtract(existingProbe.Key), existingProbe.Key);
                        existingProbe.Value.Add(existingProbe.Key.Subtract(probe), probe);
                    }
                    Rotations[rotation].Add(probe, distances);
                }
            }

            public void CreateRotations()
            {
                foreach (Vector probe in Probes.Select(p => p.Key))
                {
                    Vector rotatedProbe = probe;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i == 12) rotatedProbe = rotatedProbe.Rotate().Turn().Rotate();
                        switch (i % 4)
                        {
                            case 0: rotatedProbe = rotatedProbe.Rotate(); AddProbe(rotatedProbe, i); break;
                            case 1:
                            case 2:
                            case 3: rotatedProbe = rotatedProbe.Turn(); AddProbe(rotatedProbe, i); break;
                        }
                    }
                }
            }

            public (int rotation, Vector? offset) FindOverlap(Scanner otherScanner)
            {
                for (int i = 0; i < otherScanner.Rotations.Count; i++)
                {
                    Dictionary<Vector, int> matches = new();

                    foreach (KeyValuePair<Vector, Dictionary<Vector, Vector>> probe in Probes)
                    {
                        foreach (KeyValuePair<Vector, Dictionary<Vector, Vector>> otherProbe in otherScanner.Rotations[i])
                        {
                            bool probesMatch = false;
                            Vector offset = probe.Key.Subtract(otherProbe.Key);
                            foreach (KeyValuePair<Vector, Vector> distance in probe.Value)
                            {
                                if (otherProbe.Value.ContainsKey(distance.Key))
                                {
                                    probesMatch = true;
                                    break;
                                }
                            }
                            if (probesMatch)
                            {
                                if (!matches.ContainsKey(offset)) matches.Add(offset, 0);
                                matches[offset]++;
                            }
                        }
                    }

                    if (matches.Count > 0)
                    {
                        KeyValuePair<Vector, int> bestMatch = matches.OrderByDescending(m => m.Value).First();
                        if (bestMatch.Value >= 12) return (i, bestMatch.Key);
                    }
                }
                return (-1, null);
            }
        }

        readonly List<Scanner> MatchedScanners = new();
        public override string Part1()
        {
            List<Scanner> unmatchedScanners = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex scannerLine = new(@"--- scanner (\d+) ---");
                Scanner? newScanner = null;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (scannerLine.IsMatch(line))
                    {
                        if (newScanner != null)
                        {
                            newScanner!.CreateRotations();
                            unmatchedScanners.Add(newScanner);
                        }
                        newScanner = new Scanner(scannerLine.Match(line).Groups[1].Value);
                    }
                    else if (line.Length > 0)
                    {
                        int[] probe = line.Split(',').Select(p => int.Parse(p)).ToArray();
                        newScanner!.AddProbe(new Vector(probe[0], probe[1], probe[2]));
                    }
                }
                newScanner!.CreateRotations();
                unmatchedScanners.Add(newScanner!);
            }

            HashSet<Vector> uniqueProbes = new();
            while (unmatchedScanners.Count > 0)
            {
                List<Scanner> scanners = MatchedScanners.Count > 0 ? MatchedScanners : unmatchedScanners;
                for (int i = 0; i < scanners.Count; i++)
                {
                    bool matchFound = false;
                    for (int j = 0; j < unmatchedScanners.Count; j++)
                    {
                        if (scanners[i] == unmatchedScanners[j]) continue;

                        (int rotation, Vector? offset) match = scanners[i].FindOverlap(unmatchedScanners[j]);
                        if (match.rotation > -1)
                        {
                            Scanner scanner = scanners[i];
                            Scanner matchedScanner = unmatchedScanners[j];
                            if (MatchedScanners.Count == 0)
                            {
                                MatchedScanners.Add(scanner);
                                _ = unmatchedScanners.Remove(scanner);
                                foreach (Vector probe in scanner.Probes.Keys)
                                {
                                    _ = uniqueProbes.Add(probe);
                                }
                            }

                            MatchedScanners.Add(matchedScanner);
                            _ = unmatchedScanners.Remove(matchedScanner);

                            matchedScanner.SetPosition(match.offset!, match.rotation);
                            foreach (Vector probe in matchedScanner.Probes.Keys) _ = uniqueProbes.Add(probe);
                            matchFound = true;
                            break;
                        }
                    }
                    if (matchFound) break;
                }
            }

            return uniqueProbes.Count.ToString();
        }

        public override string Part2()
        {
            long largestDist = 0;
            foreach (Scanner scanner1 in MatchedScanners)
            {
                foreach (Scanner scanner2 in MatchedScanners)
                {
                    largestDist = Math.Max(largestDist, scanner1.Position!.ManhattanDistance(scanner2.Position!));
                }
            }

            return largestDist.ToString();
        }
    }
}
