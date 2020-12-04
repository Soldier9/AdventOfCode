using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day4Solver : AbstractSolver
    {
        struct Passport
        {
            public string byr;
            public string iyr;
            public string eyr;
            public string hgt;
            public string hcl;
            public string ecl;
            public string pid;
            public string cid;
        }

        private List<Passport> passports = new List<Passport>();

        public override string Part1()
        {
            using(var input = File.OpenText(InputFile))
            {
                Passport passport = new Passport();

                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();
                    if (line == "")
                    {
                        passports.Add(passport);
                        passport = new Passport();
                        continue;
                    }

                    string[] pairs = line.Split(' ');
                    foreach(string pair in pairs)
                    {
                        string[] parts = pair.Split(':');
                        switch(parts[0])
                        {
                            case "byr": passport.byr = parts[1]; break;
                            case "iyr": passport.iyr = parts[1]; break;
                            case "eyr": passport.eyr = parts[1]; break;
                            case "hgt": passport.hgt = parts[1]; break;
                            case "hcl": passport.hcl = parts[1]; break;
                            case "ecl": passport.ecl = parts[1]; break;
                            case "pid": passport.pid = parts[1]; break;
                            case "cid": passport.cid = parts[1]; break;
                        }
                    }
                }
                passports.Add(passport);
            }

            int result = 0;
            foreach (Passport pp in passports)
            {
                if (pp.byr == null || pp.iyr == null || pp.eyr == null || pp.hgt == null || pp.hcl == null || pp.ecl == null || pp.pid == null) continue;
                result++;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            Regex hgtValid = new Regex(@"^(\d+)(cm|in)$");
            Regex hclValid = new Regex(@"^#(\d|[abcdef]){6}$");
            Regex pidValid = new Regex(@"^\d{9}$");

            int result = 0;
            foreach (Passport pp in passports)
            {
                if (pp.byr == null || pp.iyr == null || pp.eyr == null || pp.hgt == null || pp.hcl == null || pp.ecl == null || pp.pid == null) continue;

                int tmp = 0;
                if (!(int.TryParse(pp.byr, out tmp) && tmp >= 1920 && tmp <= 2002)) continue;
                if (!(int.TryParse(pp.iyr, out tmp) && tmp >= 2010 && tmp <= 2020)) continue;
                if (!(int.TryParse(pp.eyr, out tmp) && tmp >= 2020 && tmp <= 2030)) continue;

                var hgtMatch = hgtValid.Match(pp.hgt);
                if (hgtMatch == Match.Empty) continue;
                if (hgtMatch.Groups[2].Value == "cm" && !(int.Parse(hgtMatch.Groups[1].Value) >= 150 && int.Parse(hgtMatch.Groups[1].Value) <= 193)) continue;
                if (hgtMatch.Groups[2].Value == "in" && !(int.Parse(hgtMatch.Groups[1].Value) >= 59 && int.Parse(hgtMatch.Groups[1].Value) <= 76)) continue;

                if (!hclValid.IsMatch(pp.hcl)) continue;
                switch(pp.ecl)
                {
                    case "amb":
                    case "blu":
                    case "brn":
                    case "gry":
                    case "grn":
                    case "hzl":
                    case "oth": break;
                    default: continue;
                }

                if (!pidValid.IsMatch(pp.pid)) continue;

                result++;
            }

            return result.ToString();
        }
    }
}
