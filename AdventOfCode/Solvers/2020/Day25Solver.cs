using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solvers.Year2020
{
    class Day25Solver : AbstractSolver
    {
        
        public override string Part1()
        {
            Dictionary<BigInteger, BigInteger> pkeys = new Dictionary<BigInteger, BigInteger>();
            int subject = 7;
            int modulus = 20201227;

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    pkeys.Add(long.Parse(input.ReadLine()), 0);
                }
            }

            int loopSizesFound = 0;
            for(BigInteger loopSize = 1; loopSizesFound < 2; loopSize++)
            {
                BigInteger currentPublicKey = BigInteger.ModPow(subject, loopSize, modulus);
                if (pkeys.ContainsKey(currentPublicKey))
                {
                    pkeys[currentPublicKey] = loopSize;
                    loopSizesFound++;
                }
            }

            BigInteger encKey = BigInteger.ModPow(pkeys.Last().Key, pkeys.First().Value, modulus);
            return encKey.ToString();
        }

        public override string Part2()
        {
            return "No solution needed!";
        }
    }
}
