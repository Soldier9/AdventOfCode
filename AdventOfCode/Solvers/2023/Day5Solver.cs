using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day5Solver : AbstractSolver
    {
        List<(long source, long dest, long size)> seedToSoil = new();
        List<(long source, long dest, long size)> soilToFertilizer = new();
        List<(long source, long dest, long size)> fertilizerToWater = new();
        List<(long source, long dest, long size)> waterToLight = new();
        List<(long source, long dest, long size)> lightToTemperature = new();
        List<(long source, long dest, long size)> temperatureToHumidity = new();
        List<(long source, long dest, long size)> humidityToLocation = new();

        public override string Part1()
        {
            List<long> seeds = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parseSeeds = new Regex(@"\d+");
                List<(long source, long dest, long size)> currentMap = new();

                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.StartsWith("seeds:")) foreach (Match match in parseSeeds.Matches(line)) seeds.Add(long.Parse(match.Value));
                    else
                    {
                        switch (line)
                        {
                            case "seed-to-soil map:": currentMap = seedToSoil; continue;
                            case "soil-to-fertilizer map:": currentMap = soilToFertilizer; continue;
                            case "fertilizer-to-water map:": currentMap = fertilizerToWater; continue;
                            case "water-to-light map:": currentMap = waterToLight; continue;
                            case "light-to-temperature map:": currentMap = lightToTemperature; continue;
                            case "temperature-to-humidity map:": currentMap = temperatureToHumidity; continue;
                            case "humidity-to-location map:": currentMap = humidityToLocation; continue;
                            case "": continue;
                        }

                        string[] numbers = line.Split(' ');
                        (long source, long dest, long size) newEntry = new();
                        newEntry.source = long.Parse(numbers[1]);
                        newEntry.dest = long.Parse(numbers[0]);
                        newEntry.size = long.Parse(numbers[2]);
                        currentMap.Add(newEntry);
                    }
                }
            }

            long result = long.MaxValue;
            foreach (long seed in seeds)
            {
                long value = seed;
                value = Translate(value, seedToSoil);
                value = Translate(value, soilToFertilizer);
                value = Translate(value, fertilizerToWater);
                value = Translate(value, waterToLight);
                value = Translate(value, lightToTemperature);
                value = Translate(value, temperatureToHumidity);
                value = Translate(value, humidityToLocation);
                result = Math.Min(result, value);
            }
            return result.ToString();
        }

        public long Translate(long value, List<(long source, long dest, long size)> maps)
        {
            foreach (var map in maps)
            {
                if (value >= map.source && value - map.source < map.size) return map.dest + (value - map.source);
            }
            return value;
        }

        public override string Part2()
        {
            List<(long start, long end)> seedRanges = new();

            Regex parseSeeds = new Regex(@"\d+");
            using (StreamReader input = File.OpenText(InputFile))
            {
                string line = input.ReadLine()!;
                MatchCollection matches = parseSeeds.Matches(line);
                for (int i = 0; i < matches.Count; i += 2)
                {
                    seedRanges.Add((long.Parse(matches[i].Value), long.Parse(matches[i].Value) + long.Parse(matches[i + 1].Value)));
                }
            }

            long result = long.MaxValue;
            foreach ((long start, long end) seedRange in seedRanges)
            {
                List<(long start, long end)> valueRanges = new();
                valueRanges.Add(seedRange);
                valueRanges = TranslateRange(valueRanges, seedToSoil);
                valueRanges = TranslateRange(valueRanges, soilToFertilizer);
                valueRanges = TranslateRange(valueRanges, fertilizerToWater);
                valueRanges = TranslateRange(valueRanges, waterToLight);
                valueRanges = TranslateRange(valueRanges, lightToTemperature);
                valueRanges = TranslateRange(valueRanges, temperatureToHumidity);
                valueRanges = TranslateRange(valueRanges, humidityToLocation);
                result = Math.Min(result, valueRanges.OrderBy(n => n.start).First().start);
            }

            return result.ToString();
        }

        public List<(long start, long end)> TranslateRange(List<(long start, long end)> sourceRange, List<(long source, long dest, long size)> maps)
        {
            List<(long start, long end)> resultRange = new();
            
            while(sourceRange.Count > 0) 
            {
                (long start, long end) range = sourceRange[0];
                sourceRange.RemoveAt(0);

                bool mappingFound = false;
                foreach ((long source, long dest, long size) map in maps)
                {
                    if (range.end < map.source || range.start > map.source + map.size - 1) continue;
                    if (range.start >= map.source && range.end <= map.source + map.size - 1)
                    {
                        resultRange = AddRange(resultRange, (range.start + map.dest - map.source, range.end + map.dest - map.source));
                        mappingFound = true;
                        break;
                    }
                    if (range.start <= map.source && range.end >= map.source + map.size - 1)
                    {
                        resultRange = AddRange(resultRange, (map.dest, map.dest + map.size - 1));
                        sourceRange = AddRange(sourceRange, (range.start, map.source - 1));
                        sourceRange = AddRange(sourceRange, (map.source + map.size, range.end));
                        mappingFound = true;
                        break;
                    }
                    if (range.start >= map.source && range.start <= map.source + map.size - 1)
                    {
                        resultRange = AddRange(resultRange, (range.start + map.dest - map.source, map.dest + map.size - 1));
                        sourceRange = AddRange(sourceRange, (map.source + map.size, range.end));
                        mappingFound = true;
                        break;
                    }
                    if (range.end >= map.source && range.end <= map.source + map.size - 1)
                    {
                        resultRange = AddRange(resultRange, (map.dest, range.end + map.dest - map.source));
                        sourceRange = AddRange(sourceRange, (range.start, map.source - 1));
                        mappingFound = true;
                        break;
                    }
                }

                if (!mappingFound) resultRange = AddRange(resultRange, range);
            }
            return resultRange.ToList();
        }

        public List<(long start, long end)> AddRange(List<(long start, long end)> ranges, (long start, long end) newRange)
        {
            List<(long start, long end)> result = ranges;
            for(int i = 0; i < result.Count; i++)
            {
                (long start, long end) existingRange = result[i];
                if (newRange.start >= existingRange.start && newRange.end <= existingRange.end) return result;
                else if(newRange.start <= existingRange.start && newRange.end >= existingRange.end) {
                    result.RemoveAt(i--);
                    result.Add(newRange);
                    return result;
                }
            }
            result.Add(newRange);
            return result;
        }
    }
}
