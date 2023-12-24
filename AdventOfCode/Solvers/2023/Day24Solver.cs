using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day24Solver : AbstractSolver
    {
        List<((long x, long y, long z) position, (long x, long y, long z) vector)> hails = new();
        public override string Part1()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parseNumbers = new(@"-?\d+");
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split('@');
                    MatchCollection m = parseNumbers.Matches(line[0]);
                    (long x, long y, long z) position = (long.Parse(m[0].Value), long.Parse(m[1].Value), long.Parse(m[2].Value));
                    m = parseNumbers.Matches(line[1]);
                    (long x, long y, long z) vector = (long.Parse(m[0].Value), long.Parse(m[1].Value), long.Parse(m[2].Value));
                    hails.Add((position, vector));
                }
            }

            int result = 0;
            for (int i = 0; i < hails.Count; i++)
            {
                for (int j = i + 1; j < hails.Count; j++)
                {
                    (double x, double y)? intersection = IntersectsAt(hails[i], hails[j]);
                    if (intersection == null) continue;

                    if (intersection.Value.x >= 200000000000000 && intersection.Value.x <= 400000000000000 &&
                        intersection.Value.y >= 200000000000000 && intersection.Value.y <= 400000000000000)
                    {
                        result++;
                    }
                }
            }

            return result.ToString();
        }

        static (double x, double y)? IntersectsAt(((long x, long y, long z) position, (long x, long y, long z) vector) hail1, ((long x, long y, long z) position, (long x, long y, long z) vector) hail2)
        {
            (double x, double y) result;

            double h1 = (double)hail1.vector.y / (double)hail1.vector.x;
            double h2 = (double)hail2.vector.y / (double)hail2.vector.x;

            double b1 = (double)hail1.position.y - (h1 * (double)hail1.position.x);
            double b2 = (double)hail2.position.y - (h2 * (double)hail2.position.x);

            result.x = (b2 - b1) / (h1 - h2);
            result.y = h1 * result.x + b1;

            if (((result.x - hail1.position.x) / (double)hail1.vector.x < 0) ||
                ((result.x - hail2.position.x) / (double)hail2.vector.x < 0)) return null;

            return result;
        }

        public override string Part2()
        {

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;

                }
            }

            return "Nope".ToString();
        }
    }
}
