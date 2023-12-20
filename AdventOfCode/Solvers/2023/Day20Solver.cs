namespace AdventOfCode.Solvers.Year2023
{
    class Day20Solver : AbstractSolver
    {
        abstract class Module
        {
            public string name;
            public static long lowPulses = 0;
            public static long highPulses = 0;

            public static Dictionary<string, Module> AllModules = new();
            public static Queue<(Module module, bool pulseType)> ProcessingQueue = new();

            protected List<Module> Destinations = new();

            readonly string[] destinations;

            public override string ToString() => name;

            public Module(string name, string[] destinations)
            {
                this.destinations = destinations;
                this.name = name;
            }

            public void MatchDestinations()
            {
                foreach (string destination in destinations)
                {
                    // If receiver is not explicitly defined in input as sender, use a dummy-broadcaster with no destinations
                    if (!AllModules.ContainsKey(destination)) AllModules.Add(destination, new Broadcaster(destination, Array.Empty<string>()));
                    
                    Destinations.Add(AllModules[destination]);
                    if (AllModules[destination] is Conjunction conjunctionDestination)
                    {
                        conjunctionDestination.Inputs[this] = false;
                    }
                }
            }

            public abstract void ReceivePulse(bool highPulse, Module sender);
            public abstract void SendPulse(bool highPulse);

            protected static void CountPulse(bool highPulse)
            {
                if (highPulse) highPulses++;
                else lowPulses++;
            }
        }
        class FlipFlop : Module
        {
            bool State = false;

            public FlipFlop(string name, string[] destinations) : base(name, destinations)
            {
            }

            public override void ReceivePulse(bool highPulse, Module sender)
            {
                if (highPulse) return;
                State = !State;
                ProcessingQueue.Enqueue((this, State));
            }

            public override void SendPulse(bool highPulse)
            {
                foreach (Module destination in Destinations)
                {
                    destination.ReceivePulse(highPulse, this);
                    CountPulse(highPulse);
                }
            }
        }

        class Conjunction : Module
        {
            public Dictionary<Module, bool> Inputs = new();

            public Conjunction(string name, string[] destinations) : base(name, destinations)
            {
            }

            public override void ReceivePulse(bool highPulse, Module sender)
            {
                Inputs[sender] = highPulse;
                ProcessingQueue.Enqueue((this, Inputs.ContainsValue(false)));
            }

            public override void SendPulse(bool highPulse)
            {
                foreach (Module destination in Destinations)
                {
                    destination.ReceivePulse(highPulse, this);
                    CountPulse(highPulse);
                }
            }
        }

        class Broadcaster : Module
        {
            public Broadcaster(string name, string[] destinations) : base(name, destinations)
            {
            }

            public override void SendPulse(bool highPulse)
            {
                foreach (Module destination in Destinations)
                {
                    destination.ReceivePulse(highPulse, this);
                    CountPulse(highPulse);
                }
            }

            public override void ReceivePulse(bool highPulse, Module sender)
            {
                ProcessingQueue.Enqueue((this, highPulse));
            }
        }


        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(" -> ");
                    if (line[0] == "broadcaster")
                    {
                        Broadcaster broadcaster = new(line[0], line[1].Split(", "));
                        Module.AllModules.Add(line[0], broadcaster);
                    }
                    else if (line[0][0] == '%')
                    {
                        FlipFlop flipFlop = new(line[0], line[1].Split(", "));
                        Module.AllModules.Add(line[0][1..], flipFlop);
                    }
                    else if (line[0][0] == '&')
                    {
                        Conjunction conjunction = new(line[0], line[1].Split(", "));
                        Module.AllModules.Add(line[0][1..], conjunction);
                    }
                }
                foreach (Module module in Module.AllModules.Values.ToList()) module.MatchDestinations();
            }

            for (int i = 0; i < 1000; i++)
            {
                Module.lowPulses++;
                Module.AllModules["broadcaster"].ReceivePulse(false, Module.AllModules["broadcaster"]);
                while (Module.ProcessingQueue.Count > 0)
                {
                    (Module module, bool pulseType) nextSender = Module.ProcessingQueue.Dequeue();
                    nextSender.module.SendPulse(nextSender.pulseType);
                }
            }

            return (Module.lowPulses * Module.highPulses).ToString();
        }

        public override string Part2()
        {
            Module.AllModules.Clear();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(" -> ");
                    if (line[0] == "broadcaster")
                    {
                        Broadcaster broadcaster = new(line[0], line[1].Split(", "));
                        Module.AllModules.Add(line[0], broadcaster);
                    }
                    else if (line[0][0] == '%')
                    {
                        FlipFlop flipFlop = new(line[0][1..], line[1].Split(", "));
                        Module.AllModules.Add(line[0][1..], flipFlop);
                    }
                    else if (line[0][0] == '&')
                    {
                        Conjunction conjunction = new(line[0][1..], line[1].Split(", "));
                        Module.AllModules.Add(line[0][1..], conjunction);
                    }
                }
                List<Module> modules = Module.AllModules.Values.ToList();
                foreach (Module module in modules) module.MatchDestinations();
            }

            // This probably needs adjusting for other inputs
            Dictionary<string, long> dfInputs = new()
            {
                { "xl", -1 },
                { "ln", -1 },
                { "xp", -1 },
                { "gp", -1 }
            };

            long result = 1;
            for (long i = 0;  dfInputs.Count > 0; i++)
            {
                Module.AllModules["broadcaster"].ReceivePulse(false, Module.AllModules["broadcaster"]);
                while (Module.ProcessingQueue.Count > 0)
                {
                    (Module module, bool pulseType) nextSender = Module.ProcessingQueue.Dequeue();
                    nextSender.module.SendPulse(nextSender.pulseType);

                    if (dfInputs.ContainsKey(nextSender.module.name) && nextSender.pulseType)
                    {
                        //Console.WriteLine(nextSender.module.name + " sent high pulse: " + (i + 1));
                        if (dfInputs[nextSender.module.name] > -1)
                        {
                            long cycleLength = i - dfInputs[nextSender.module.name];
                            dfInputs.Remove(nextSender.module.name);
                            //Console.WriteLine(nextSender.module.name + " cycle detected! Length " + cycleLength);
                            result = lcm(result, cycleLength);
                        }
                        else
                        {
                            dfInputs[nextSender.module.name] = i;
                        }
                    }
                }
            }

            return result.ToString();
        }

        private static long lcm(long x, long y)
        {
            return (x * y) / gcd(x, y);
        }

        private static long gcd(long x, long y)
        {
            long result = Math.Min(x, y);
            while (result > 0)
            {
                if (x % result == 0 && y % result == 0) break;
                result--;
            }
            return result;
        }
    }
}
