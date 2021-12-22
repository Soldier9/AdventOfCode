using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2021
{
    class Day22Solver : AbstractSolver
    {
        public override string Part1()
        {
            Dictionary<(int x, int y, int z), bool> cubes = new();
            Regex parser = new(@"(on|off) x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)");

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Match line = parser.Match(input.ReadLine()!);
                    (int min, int max) xRange = (int.Parse(line.Groups[2].Value), int.Parse(line.Groups[3].Value));
                    (int min, int max) yRange = (int.Parse(line.Groups[4].Value), int.Parse(line.Groups[5].Value));
                    (int min, int max) zRange = (int.Parse(line.Groups[6].Value), int.Parse(line.Groups[7].Value));

                    bool state = line.Groups[1].Value == "on";

                    for (int x = Math.Max(-50, xRange.min); x <= Math.Min(xRange.max, 50); x++)
                    {
                        for (int y = Math.Max(-50, yRange.min); y <= Math.Min(yRange.max, 50); y++)
                        {
                            for (int z = Math.Max(-50, zRange.min); z <= Math.Min(zRange.max, 50); z++)
                            {
                                if (!cubes.ContainsKey((x, y, z))) cubes.Add((x, y, z), state);
                                else cubes[(x, y, z)] = state;
                            }
                        }
                    }
                }
            }

            return cubes.Count(c => c.Value).ToString();
        }

        public override string Part2()
        {
            Dictionary<((int x, int y, int z) min, (int x, int y, int z) max), long> cubes = new();
            Regex parser = new(@"(on|off) x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)");

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Match line = parser.Match(input.ReadLine()!);
                    (int min, int max) xRange = (int.Parse(line.Groups[2].Value), int.Parse(line.Groups[3].Value));
                    (int min, int max) yRange = (int.Parse(line.Groups[4].Value), int.Parse(line.Groups[5].Value));
                    (int min, int max) zRange = (int.Parse(line.Groups[6].Value), int.Parse(line.Groups[7].Value));

                    bool state = line.Groups[1].Value == "on";

                    ((int x, int y, int z) min, (int x, int y, int z) max) newCube;
                    newCube.min.x = xRange.min;
                    newCube.min.y = yRange.min;
                    newCube.min.z = zRange.min;
                    newCube.max.x = xRange.max + 1;
                    newCube.max.y = yRange.max + 1;
                    newCube.max.z = zRange.max + 1;

                    Dictionary<((int x, int y, int z) min, (int x, int y, int z) max), long> newCubes = new();
                    foreach (KeyValuePair<((int x, int y, int z) min, (int x, int y, int z) max), long> cube in cubes)
                    {
                        foreach (((int x, int y, int z) min, (int x, int y, int z) max) splitCube in SplitCube(cube.Key, newCube))
                        {
                            if (!Contains(newCube, splitCube)) _ = newCubes.TryAdd(splitCube, Volume(splitCube));
                        }
                    }
                    if (state) _ = newCubes.TryAdd(newCube, Volume(newCube));

                    cubes = newCubes;
                }
            }

            long result = cubes.Sum(c => c.Value);
            return result.ToString();
        }

        private static long Volume(((int x, int y, int z) min, (int x, int y, int z) max) cube)
        {
            long x = Math.Abs(cube.max.x - cube.min.x);
            long y = Math.Abs(cube.max.y - cube.min.y);
            long z = Math.Abs(cube.max.z - cube.min.z);

            return x * y * z;
        }

        private static bool Contains(((int x, int y, int z) min, (int x, int y, int z) max) cube1, ((int x, int y, int z) min, (int x, int y, int z) max) cube2)
        {
            return (cube1.min.x <= cube2.min.x && cube1.min.y <= cube2.min.y && cube1.min.z <= cube2.min.z &&
                cube1.max.x >= cube2.max.x && cube1.max.y >= cube2.max.y && cube1.max.z >= cube2.max.z);
        }

        private static List<((int x, int y, int z) min, (int x, int y, int z) max)> SplitCube(((int x, int y, int z) min, (int x, int y, int z) max) cube1, ((int x, int y, int z) min, (int x, int y, int z) max) cube2)
        {
            List<((int x, int y, int z) min, (int x, int y, int z) max)> result = new();
            if (cube1.max.x < cube2.min.x || cube1.max.y < cube2.min.y || cube1.max.z < cube2.min.z || cube1.min.x >= cube2.max.x || cube1.min.y >= cube2.max.y || cube1.min.z >= cube2.max.z)
            {   // No overlap
                result.Add(cube1);
                return result;
            }

            if (cube1.min.x < cube2.min.x && cube2.min.x <= cube1.max.x) result.Add((cube1.min, (cube2.min.x, cube1.max.y, cube1.max.z)));
            if (cube1.min.x <= cube2.max.x && cube2.max.x < cube1.max.x) result.Add(((cube2.max.x, cube1.min.y, cube1.min.z), cube1.max));
            ((int x, int y, int z) min, (int x, int y, int z) max) splitAgain = ((Math.Max(cube2.min.x, cube1.min.x), cube1.min.y, cube1.min.z), (Math.Min(cube2.max.x, cube1.max.x), cube1.max.y, cube1.max.z));

            if (splitAgain.min.y < cube2.min.y && cube2.min.y <= splitAgain.max.y) result.Add((splitAgain.min, (splitAgain.max.x, cube2.min.y, splitAgain.max.z)));
            if (splitAgain.min.y <= cube2.max.y && cube2.max.y < splitAgain.max.y) result.Add(((splitAgain.min.x, cube2.max.y, splitAgain.min.z), splitAgain.max));
            splitAgain = ((splitAgain.min.x, Math.Max(cube2.min.y, splitAgain.min.y), splitAgain.min.z), (splitAgain.max.x, Math.Min(cube2.max.y, splitAgain.max.y), splitAgain.max.z));

            if (splitAgain.min.z < cube2.min.z && cube2.min.z <= splitAgain.max.z) result.Add((splitAgain.min, (splitAgain.max.x, splitAgain.max.y, cube2.min.z)));
            if (splitAgain.min.z <= cube2.max.z && cube2.max.z < splitAgain.max.z) result.Add(((splitAgain.min.x, splitAgain.min.y, cube2.max.z), splitAgain.max));
            result.Add(((splitAgain.min.x, splitAgain.min.y, Math.Max(cube2.min.z, splitAgain.min.z)), (splitAgain.max.x, splitAgain.max.y, Math.Min(cube2.max.z, splitAgain.max.z))));

            return result;
        }
    }
}
