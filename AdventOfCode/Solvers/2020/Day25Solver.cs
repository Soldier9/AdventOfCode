using System.Numerics;

namespace AdventOfCode.Solvers.Year2020
{
    class Day25Solver : AbstractSolver
    {

        public override string Part1()
        {
            int subject = 7;
            int modulus = 20201227;
            int publicKey1;
            int publicKey2;

            using (StreamReader input = File.OpenText(InputFile))
            {
                publicKey1 = int.Parse(input.ReadLine()!);
                publicKey2 = int.Parse(input.ReadLine()!);
            }

            int loopSize = 1;
            while (true)
            {
                BigInteger tmp = BigInteger.ModPow(subject, loopSize, modulus);
                if (tmp == publicKey1) return BigInteger.ModPow(publicKey2, loopSize, modulus).ToString();
                if (tmp == publicKey2) return BigInteger.ModPow(publicKey1, loopSize, modulus).ToString();
                loopSize++;
            }
        }

        public override string Part2()
        {
            return "No solution needed!";
        }
    }
}
