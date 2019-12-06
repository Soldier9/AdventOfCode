using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day06Solver : AbstractSolver
    {
        class Celestial
        {
            public static HashSet<Celestial> AllCelestials = new HashSet<Celestial>();

            public string ID;
            string OrbittingString;
            
            public Celestial Orbitting;
            public HashSet<Celestial> OrbittedBy = new HashSet<Celestial>();

            public Celestial(string id, string orbitting)
            {
                ID = id;
                OrbittingString = orbitting;
                AllCelestials.Add(this);

                if (AllCelestials.Any(c => c.ID == orbitting))
                {
                    Orbitting = AllCelestials.Single(c => c.ID == orbitting);
                    Orbitting.OrbittedBy.Add(this);
                }
                

                foreach (Celestial orbitter in AllCelestials.Where(c => c.OrbittingString == ID))
                {
                    orbitter.Orbitting = this;
                    OrbittedBy.Add(orbitter);
                }
            }

            public static int GetTotalNumberOfOrbits()
            {
                int orbits = 0;
                
                foreach(Celestial celestial in AllCelestials)
                {
                    var test = celestial;
                    while(test.Orbitting != null)
                    {
                        orbits++;
                        test = test.Orbitting;
                    }
                    if(test.Orbitting == null)
                    {
                        orbits++;
                    }
                }

                return orbits;
            }
        }


        public override string Part1()
        {
            

            using(var input = File.OpenText(InputFile))
            {
                while(!input.EndOfStream)
                {
                    string line = input.ReadLine();

                    new Celestial(line.Split(')')[1], line.Split(')')[0]);
                }
            }

            return Celestial.GetTotalNumberOfOrbits().ToString();
        }

        public override string Part2()
        {
            Celestial me = Celestial.AllCelestials.Single(c => c.ID == "YOU");
            Dictionary<Celestial, int> myPathToSun = new Dictionary<Celestial, int>();

            int transfers = 0;
            while(me.Orbitting != null)
            {
                myPathToSun.Add(me.Orbitting,++transfers);
                me = me.Orbitting;
            }

            Celestial santa = Celestial.AllCelestials.Single(c => c.ID == "SAN");
            Dictionary<Celestial, int> santasPathToSun = new Dictionary<Celestial, int>();

            transfers = 0;
            while (santa.Orbitting != null)
            {
                santasPathToSun.Add(santa.Orbitting, ++transfers);
                santa = santa.Orbitting;
            }

            foreach(KeyValuePair<Celestial,int> c in myPathToSun.OrderBy(c => c.Value))
            {
                if(santasPathToSun.ContainsKey(c.Key))
                {
                    return (c.Value + santasPathToSun[c.Key] - 2).ToString();
                }
            }

            return "FUCK!";
        }
    }
}
