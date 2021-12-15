namespace AdventOfCode.Solvers.Year2020
{
    class Day16Solver : AbstractSolver
    {
        class Rule
        {
            public string? Name;
            public (int, int) Range1;
            public (int, int) Range2;
            public int Fieldnumber;
            public List<int> PotentialFieldNums = new();
        }

        readonly List<Rule> Rules = new();
        readonly List<List<int>> NearbyTickets = new();
        readonly List<List<int>> ValidNearbyTickets = new();
        List<int> MyTicket = new();
        int ValuesOnTicket = 0;
        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingRules = true;
                bool parsingYourTicket = false;
                bool parsingNearbyTickets = false;

                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Length == 0) continue;
                    if (line.Length == 1)
                    {
                        if (line[0] == "your ticket")
                        {
                            parsingRules = false;
                            parsingYourTicket = true;
                            continue;
                        }
                        if (line[0] == "nearby tickets")
                        {
                            parsingYourTicket = false;
                            parsingNearbyTickets = true;
                            continue;
                        }
                    }

                    if (parsingRules)
                    {
                        string[] ranges = line[1].Split(new string[] { " or " }, StringSplitOptions.None);
                        string[] range1 = ranges[0].Split('-');
                        string[] range2 = ranges[1].Split('-');
                        Rules.Add(new Rule { Name = line[0], Range1 = (int.Parse(range1[0]), int.Parse(range1[1])), Range2 = (int.Parse(range2[0]), int.Parse(range2[1])) });
                    }
                    else if (parsingYourTicket)
                    {
                        string[] values = line[0].Split(',');
                        ValuesOnTicket = values.Length;
                        MyTicket = values.Select(v => int.Parse(v)).ToList();
                    }
                    else if (parsingNearbyTickets)
                    {
                        string[] values = line[0].Split(',');
                        NearbyTickets.Add(values.Select(v => int.Parse(v)).ToList());
                    }
                }
            }

            int result = 0;
            foreach (List<int> ticket in NearbyTickets)
            {
                bool ticketIsValid = true;
                foreach (int value in ticket)
                {
                    bool matchesSomething = false;
                    foreach (Rule rule in Rules)
                    {
                        if ((value >= rule.Range1.Item1 && value <= rule.Range1.Item2) || (value >= rule.Range2.Item1 && value <= rule.Range2.Item2))
                        {
                            matchesSomething = true;
                            break;
                        }
                    }
                    if (!matchesSomething)
                    {
                        result += value;
                        ticketIsValid = false;
                    }
                }
                if (ticketIsValid) ValidNearbyTickets.Add(ticket);
            }

            return result.ToString();
        }

        public override string Part2()
        {
            for (int j = 0; j < Rules.Count; j++)
            {
                for (int i = 0; i < ValuesOnTicket; i++)
                {
                    bool fieldNumberMatches = true;
                    foreach (List<int> ticket in ValidNearbyTickets)
                    {
                        if (!((ticket[i] >= Rules[j].Range1.Item1 && ticket[i] <= Rules[j].Range1.Item2) || (ticket[i] >= Rules[j].Range2.Item1 && ticket[i] <= Rules[j].Range2.Item2)))
                        {
                            fieldNumberMatches = false;
                            break;
                        }
                    }
                    if (fieldNumberMatches)
                    {
                        Rules[j].PotentialFieldNums.Add(i);
                    }
                }
            }

            int fieldsAssigned = 0;
            while (fieldsAssigned < ValuesOnTicket)
            {
                Rule currentRule = Rules.Where(r => r.PotentialFieldNums.Count == 1).First();
                currentRule.Fieldnumber = currentRule.PotentialFieldNums[0];
                foreach (Rule rule in Rules)
                {
                    _ = rule.PotentialFieldNums.Remove(currentRule.Fieldnumber);
                }
                fieldsAssigned++;
            }

            long result = 1;
            foreach (Rule rule in Rules.Where(r => r.Name!.StartsWith("departure")))
            {
                result *= MyTicket[rule.Fieldnumber];
            }

            return result.ToString();
        }
    }
}
