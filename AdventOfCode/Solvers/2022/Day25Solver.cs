using System.Numerics;
using System.Text;

namespace AdventOfCode.Solvers.Year2022
{
    class Day25Solver : AbstractSolver
    {
        List<string> snafus = new();
        List<BigInteger> decs = new();

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    snafus.Add(input.ReadLine()!);
                }
            }

            foreach (string snafu in snafus)
            {
                BigInteger dec = 0;
                BigInteger mult = 1;
                for (int i = snafu.Length - 1; i > -1; i--)
                {
                    int sdec = snafu2dec(snafu[i]);
                    dec += mult * sdec;
                    mult *= 5;
                }
                decs.Add(dec);
            }

            BigInteger decSum = 0;
            foreach (BigInteger dec in decs) decSum += dec;

            return dec2snafu(decSum);
        }

        int snafu2dec(char s)
        {
            return s switch
            {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2,
                _ => throw new Exception()
            };
        }

        string dec2snafu(BigInteger dec)
        {
            StringBuilder sb = new();
            while (dec > 0)
            {
                int d = (int)(dec % 5);
                char snafu = d switch
                {
                    4 => '-',
                    3 => '=',
                    2 => '2',
                    1 => '1',
                    0 => '0',
                    _ => throw new Exception()
                };
                _ = sb.Append(snafu);
                dec -= snafu2dec(snafu);
                dec /= 5;
            }

            return new(sb.ToString().Reverse().ToArray());
        }

        public override string Part2()
        {
            return "This is free!".ToString();
        }
    }
}
