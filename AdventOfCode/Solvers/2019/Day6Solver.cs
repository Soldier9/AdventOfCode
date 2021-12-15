namespace AdventOfCode.Solvers.Year2019
{
    class Day6Solver : AbstractSolver
    {
        class Celestial
        {
            public static HashSet<Celestial> AllCelestials = new();

            public readonly string ID;
            readonly string OrbitsString;

            public Celestial? Orbits;

            public Celestial(string id, string orbitting)
            {
                ID = id;
                OrbitsString = orbitting;
                _ = AllCelestials.Add(this);

                if (AllCelestials.Any(c => c.ID == orbitting))
                {
                    Orbits = AllCelestials.Single(c => c.ID == orbitting);
                }


                foreach (Celestial orbitter in AllCelestials.Where(c => c.OrbitsString == ID))
                {
                    orbitter.Orbits = this;
                }
            }

            public static int GetTotalNumberOfOrbits()
            {
                int orbits = 0;

                foreach (Celestial celestial in AllCelestials)
                {
                    Celestial test = celestial;
                    while (test.Orbits != null)
                    {
                        orbits++;
                        test = test.Orbits;
                    }
                    // There is no actual root-node in the graph, so Orbits will be null for the ones orbitting the root-node, but we need to count those as well!
                    if (test.Orbits == null) orbits++;
                }

                return orbits;
            }
        }


        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    _ = new Celestial(line.Split(')')[1], line.Split(')')[0]);
                }
            }

            return Celestial.GetTotalNumberOfOrbits().ToString();
        }

        static Dictionary<Celestial, int> GetPathToSun(Celestial body)
        {
            Dictionary<Celestial, int> pathToSun = new();
            Celestial currentBody = body;

            int transfers = 0;
            while (currentBody.Orbits != null)
            {
                pathToSun.Add(currentBody.Orbits, ++transfers);
                currentBody = currentBody.Orbits;
            }

            return pathToSun;
        }

        public override string Part2()
        {
            Celestial me = Celestial.AllCelestials.Single(c => c.ID == "YOU");
            Dictionary<Celestial, int> myPathToSun = GetPathToSun(me);

            Celestial santa = Celestial.AllCelestials.Single(c => c.ID == "SAN");
            Dictionary<Celestial, int> santasPathToSun = GetPathToSun(santa);

            foreach (KeyValuePair<Celestial, int> c in myPathToSun.OrderBy(c => c.Value))
            {
                if (santasPathToSun.ContainsKey(c.Key)) return (c.Value + santasPathToSun[c.Key] - 2).ToString();
            }

            return "Path not found!";
        }
    }
}
