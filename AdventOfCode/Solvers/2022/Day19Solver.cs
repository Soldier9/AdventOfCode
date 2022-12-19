using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2022
{
    class Day19Solver : AbstractSolver
    {
        class Factory
        {
            public class Materials
            {
                public int Ore;
                public int Clay;
                public int Obsidian;
                public int Geode;

                public static bool operator >(Materials? x, Materials? y)
                {
                    if (x!.Ore >= y!.Ore && x.Clay >= y.Clay && x.Obsidian >= y.Obsidian && x.Geode >= y.Geode) return true;
                    return false;
                }

                public static bool operator <(Materials? x, Materials? y)
                {
                    if (x!.Ore >= y!.Ore && x.Clay >= y.Clay && x.Obsidian >= y.Obsidian && x.Geode >= y.Geode) return false;
                    return true;
                }

                public static Materials operator -(Materials? x, Materials? y)
                {
                    Materials materials = new();
                    materials.Ore = x!.Ore - y!.Ore;
                    materials.Clay = x!.Clay - y!.Clay;
                    materials.Obsidian = x!.Obsidian - y!.Obsidian;
                    materials.Geode = x!.Geode - y!.Geode;
                    return materials;
                }

                public static Materials operator +(Materials? x, Materials? y)
                {
                    Materials materials = new();
                    materials.Ore = x!.Ore + y!.Ore;
                    materials.Clay = x!.Clay + y!.Clay;
                    materials.Obsidian = x!.Obsidian + y!.Obsidian;
                    materials.Geode = x!.Geode + y!.Geode;
                    return materials;
                }
            }

            public Task<int>? task;

            public int num;
            Materials OreRobotCost = new();
            Materials ClayRobotCost = new();
            Materials ObsidianRobotCost = new();
            Materials GeodeRobotCost = new();

            int OreRobots = 1;
            int ClayRobots = 0;
            int ObsidianRobots = 0;
            int GeodeRobots = 0;

            int MaxOreRobots = 0;
            int MaxClayRobots = 0;
            int MaxObsidianRobots = 0;

            Materials Inventory = new();

            static Regex bpParser = new(@"(\d+):.* (\d+) ore.* (\d+) ore.* (\d+) ore.* (\d+) clay.* (\d+) ore.* (\d+) obsidian.*");
            public Factory(string bp)
            {
                Match m = bpParser.Match(bp);
                this.num = int.Parse(m.Groups[1].Value!);
                this.OreRobotCost.Ore = int.Parse(m.Groups[2].Value!);
                this.ClayRobotCost.Ore = int.Parse(m.Groups[3].Value!);
                this.ObsidianRobotCost.Ore = int.Parse(m.Groups[4].Value!);
                this.ObsidianRobotCost.Clay = int.Parse(m.Groups[5].Value!);
                this.GeodeRobotCost.Ore = int.Parse(m.Groups[6].Value!);
                this.GeodeRobotCost.Obsidian = int.Parse(m.Groups[7].Value!);

                MaxOreRobots = Math.Max(Math.Max(Math.Max(OreRobotCost.Ore, ClayRobotCost.Ore), ObsidianRobotCost.Ore), GeodeRobotCost.Ore);
                MaxClayRobots = Math.Max(Math.Max(Math.Max(OreRobotCost.Clay, ClayRobotCost.Clay), ObsidianRobotCost.Clay), GeodeRobotCost.Clay);
                MaxObsidianRobots = Math.Max(Math.Max(Math.Max(OreRobotCost.Obsidian, ClayRobotCost.Obsidian), ObsidianRobotCost.Obsidian), GeodeRobotCost.Obsidian);
            }

            public Factory(Factory blueprintFrom, Materials inventory, int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots)
            {
                num = blueprintFrom.num;

                OreRobotCost = blueprintFrom.OreRobotCost;
                ClayRobotCost = blueprintFrom.ClayRobotCost;
                ObsidianRobotCost = blueprintFrom.ObsidianRobotCost;
                GeodeRobotCost = blueprintFrom.GeodeRobotCost;

                MaxOreRobots = blueprintFrom.MaxOreRobots;
                MaxClayRobots = blueprintFrom.MaxClayRobots;
                MaxObsidianRobots = blueprintFrom.MaxObsidianRobots;

                Inventory = inventory;
                OreRobots = oreRobots;
                ClayRobots = clayRobots;
                ObsidianRobots = obsidianRobots;
                GeodeRobots = geodeRobots;
            }

            static ConcurrentDictionary<(int bpId, int ore, int clay, int obsidian, int geodes, int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots, int minsLeft), int> memo = new();
            public int Build(int mins)
            {
                if (Inventory.Ore > mins * MaxOreRobots) Inventory.Ore = mins * MaxOreRobots;
                if (Inventory.Clay > mins * MaxClayRobots) Inventory.Clay = mins * MaxClayRobots;
                if (Inventory.Obsidian > mins * MaxObsidianRobots) Inventory.Obsidian = mins * MaxObsidianRobots;
                (int bpId, int ore, int clay, int obsidian, int geodes, int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots, int minsLef) memoKey
                    = (this.num, Inventory.Ore, Inventory.Clay, Inventory.Obsidian, Inventory.Geode, OreRobots, ClayRobots, ObsidianRobots, GeodeRobots, mins);

                if (memo.ContainsKey(memoKey)) return memo[memoKey];
                int mostGeodes = 0;
                while (mins > 0)
                {
                    Materials collected = new();
                    collected.Ore = OreRobots;
                    collected.Clay = ClayRobots;
                    collected.Obsidian = ObsidianRobots;
                    collected.Geode = GeodeRobots;

                    if (Inventory > GeodeRobotCost) mostGeodes = Math.Max(mostGeodes, (new Factory(this, Inventory - GeodeRobotCost + collected, OreRobots, ClayRobots, ObsidianRobots, GeodeRobots + 1)).Build(mins - 1));
                    else
                    {
                        if (OreRobots < MaxOreRobots && Inventory > OreRobotCost) mostGeodes = Math.Max(mostGeodes, (new Factory(this, Inventory - OreRobotCost + collected, OreRobots + 1, ClayRobots, ObsidianRobots, GeodeRobots)).Build(mins - 1));
                        if (ClayRobots < MaxClayRobots && Inventory > ClayRobotCost) mostGeodes = Math.Max(mostGeodes, (new Factory(this, Inventory - ClayRobotCost + collected, OreRobots, ClayRobots + 1, ObsidianRobots, GeodeRobots)).Build(mins - 1));
                        if (ObsidianRobots < MaxObsidianRobots && Inventory > ObsidianRobotCost) mostGeodes = Math.Max(mostGeodes, (new Factory(this, Inventory - ObsidianRobotCost + collected, OreRobots, ClayRobots, ObsidianRobots + 1, GeodeRobots)).Build(mins - 1));
                    }

                    Inventory += collected;
                    mins--;
                }
                mostGeodes = Math.Max(mostGeodes, Inventory.Geode);
                _ = memo.TryAdd(memoKey, mostGeodes);
                return mostGeodes;
            }
        }

        private static async void AwaitAllTasks(List<Factory> factories)
        {
            foreach (Factory factory in factories) _ = await factory.task!;
        }

        public override string Part1()
        {
            List<Factory> factories = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    factories.Add(new Factory(input.ReadLine()!));
                }
            }

            int result = 0;
            foreach (Factory factory in factories)
            {
                factory.task = Task<int>.Run(() => factory.Build(24));
            }
            AwaitAllTasks(factories);

            foreach (Factory factory in factories)
            {
                int geodesFromFactory = factory.task!.Result;
                result += geodesFromFactory * factory.num;
                //Console.WriteLine("Factory " + factory.num + ": " + geodesFromFactory);
            }
            return result.ToString();
        }

        public override string Part2()
        {
            List<Factory> factories = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream && factories.Count < 3)
                {
                    factories.Add(new Factory(input.ReadLine()!));
                }
            }

            int result = 1;
            foreach (Factory factory in factories)
            {
                factory.task = Task<int>.Run(() => factory.Build(32));
            }
            AwaitAllTasks(factories);

            foreach (Factory factory in factories)
            {
                int geodesFromFactory = factory.task!.Result;
                result *= geodesFromFactory;
                //Console.WriteLine("Factory " + factory.num + ": " + geodesFromFactory);
            }
            return result.ToString();
        }
    }
}
