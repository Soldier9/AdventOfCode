using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day16Solver : AbstractSolver
    {
        class Actor
        {
            public int nextActionAt;
            public Valve valve;

            public Actor(int nextActionAt, Valve valve)
            {
                this.nextActionAt = nextActionAt;
                this.valve = valve;
            }
        }
        class Valve
        {
            public static Dictionary<string, Valve> allValves = new();
            public static int relevantValves = 0;

            public string name;
            public int flow;
            string[] directConnections;

            public Dictionary<Valve, int> connections = new();

            public Valve(string name, int flow, string[] connections)
            {
                this.name = name;
                this.flow = flow;
                this.directConnections = connections;
                if (flow > 0) relevantValves++;
            }

            public void FindRoutes()
            {
                foreach (Valve destValve in allValves.Values.Where(v => v.flow > 0)) connections.Add(destValve, ShortestPath(destValve));
            }

            int ShortestPath(Valve target)
            {
                Dictionary<Valve, int> costs = new(allValves.Values.Select(v => new KeyValuePair<Valve, int>(v, int.MaxValue)));
                costs[this] = 0;
                PriorityQueue<Valve, int> queue = new();
                queue.Enqueue(this, 0);

                VisitNeighbors(target, costs, queue);
                return costs[target];
            }

            bool VisitNeighbors(Valve target, Dictionary<Valve, int> costs, PriorityQueue<Valve, int> queue)
            {
                if (this == target) return true;

                for (int i = 0; i < directConnections.Length; i++)
                {
                    Valve neighbor = allValves[directConnections[i]];
                    if (costs[neighbor] > costs[this] + 1)
                    {
                        costs[neighbor] = costs[this] + 1;
                        queue.Enqueue(neighbor, costs[neighbor]);
                    }
                }

                while (queue.Count > 0 && !queue.Dequeue().VisitNeighbors(target, costs, queue)) { }
                return false;
            }

            public static (int flow, Dictionary<string, int> openedValves) OpenValves(List<Actor> actors, int minsLeft, int flowReleased, int currentFlow, Dictionary<string, int> openValves)
            {
                bool valveWasOpened = false;
                foreach (Actor actor in actors.Where(a => a.nextActionAt == minsLeft && a.valve.flow > 0 && !openValves.ContainsKey(a.valve.name)))
                {
                    openValves.Add(actor.valve.name, minsLeft);
                    valveWasOpened = true;
                }

                if (valveWasOpened)
                {
                    minsLeft--;
                    flowReleased += currentFlow;

                    foreach (Actor actor in actors.Where(a => openValves.ContainsKey(a.valve.name)))
                    {
                        currentFlow += actor.valve.flow;
                        actor.nextActionAt--;
                    }

                    if (openValves.Count == relevantValves)
                    {
                        while (minsLeft > 0)
                        {
                            minsLeft--;
                            flowReleased += currentFlow;
                        }
                    }
                    if (minsLeft == 0) return (flowReleased, openValves);
                }

                (int flow, Dictionary<string, int> openedValves) bestResult = (0, new());
                foreach (Actor actor in actors.Where(a => a.nextActionAt == minsLeft))
                {
                    bool nextValveFound = false;
                    foreach (KeyValuePair<Valve, int> nextValve in actor.valve.connections.Where(c => !openValves.ContainsKey(c.Key.name) && c.Value + 1 < minsLeft && !actors.Any(a => a.valve == c.Key)))
                    {
                        nextValveFound = true;
                        List<Actor> nextActors = new();
                        foreach(Actor cloneActor in actors.Where(a => a != actor)) nextActors.Add(new Actor(cloneActor.nextActionAt, cloneActor.valve));
                        nextActors.Add(new Actor(minsLeft - nextValve.Value, nextValve.Key));

                        Actor nextActor = nextActors.MaxBy(a => a.nextActionAt)!;

                        (int flow, Dictionary<string, int> openedValves) result = OpenValves(nextActors, nextActor.nextActionAt, flowReleased + (minsLeft - nextActor.nextActionAt) * currentFlow, currentFlow, new(openValves));
                        if (bestResult.flow < result.flow) bestResult = result;
                    }
                    if (!nextValveFound && actors.Count > 1)
                    {
                        List<Actor> nextActors = new();
                        foreach (Actor cloneActor in actors.Where(a => a != actor)) nextActors.Add(new Actor(cloneActor.nextActionAt, cloneActor.valve));
                        Actor nextActor = nextActors.MaxBy(a => a.nextActionAt)!;

                        (int flow, Dictionary<string, int> openedValves) result = OpenValves(nextActors, nextActor.nextActionAt, flowReleased + (minsLeft - nextActor.nextActionAt) * currentFlow, currentFlow, new(openValves));
                        if (bestResult.flow < result.flow) bestResult = result;
                    }
                }

                while (minsLeft > 0)
                {
                    minsLeft--;
                    flowReleased += currentFlow;
                }
                if (bestResult.flow < flowReleased) bestResult = (flowReleased, openValves);
                return bestResult;
            }

            public override string ToString() => this.name;
        }

        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parser = new(@"Valve (\w{2}).+=(\d+);.+valves? (.*)");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    Match parsed = parser.Match(line);
                    Valve.allValves.Add(parsed.Groups[1].Value, new Valve(parsed.Groups[1].Value, int.Parse(parsed.Groups[2].Value!), parsed.Groups[3].Value!.Split(", ")));
                }
            }
            foreach (Valve valve in Valve.allValves.Values) valve.FindRoutes();
            List<Actor> actors = new();
            actors.Add(new Actor(30, Valve.allValves["AA"]));
            return Valve.OpenValves(actors, 30, 0, 0, new()).flow.ToString();
        }


        public override string Part2()
        {

            List<Actor> actors = new();
            actors.Add(new Actor(26, Valve.allValves["AA"]));
            actors.Add(new Actor(26, Valve.allValves["AA"]));

            var res = Valve.OpenValves(actors, 26, 0, 0, new());

            return res.flow.ToString();
        }
    }
}
