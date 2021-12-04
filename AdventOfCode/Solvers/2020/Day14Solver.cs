using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day14Solver : AbstractSolver
    {
        public override string Part1()
        {
            Dictionary<long, long> memory = new Dictionary<long, long>();
            using (var input = File.OpenText(InputFile))
            {
                string mask = "";
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine().Split('=');
                    if (line[0] == "mask ")
                    {
                        mask = line[1].Trim();
                        continue;
                    }

                    Regex getAddress = new Regex(@"\[(\d+)\]");
                    int address = int.Parse(getAddress.Match(line[0]).Groups[1].Value);
                    long value = long.Parse(line[1]);

                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (mask[i] == 'X') continue;
                        value = ModifyBit(value, (mask.Length - 1) - i, mask[i] - 48);
                    }

                    if (!memory.ContainsKey(address)) memory.Add(address, value);
                    else memory[address] = value;
                }
            }

            long result = 0;
            foreach (long value in memory.Values)
            {
                result += value;
            }
            return result.ToString();
        }

        private long ModifyBit(long n, int p, long b)
        {
            long mask = (long)1 << p;
            return (n & ~mask) | ((b << p) & mask);
        }

        public override string Part2()
        {
            Dictionary<long, long> memory = new Dictionary<long, long>();
            using (var input = File.OpenText(InputFile))
            {
                string mask = "";
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine().Split('=');
                    if (line[0] == "mask ")
                    {
                        mask = line[1].Trim();
                        continue;
                    }

                    Regex getAddress = new Regex(@"\[(\d+)\]");
                    long address = long.Parse(getAddress.Match(line[0]).Groups[1].Value);
                    long value = int.Parse(line[1]);

                    List<int> floatingBits = new List<int>();
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (mask[i] == 'X') floatingBits.Add(i);
                        else if (mask[i] == '1') address = ModifyBit(address, (mask.Length - 1) - i, mask[i] - 48);
                    }

                    List<long> addresses = new List<long>();
                    addresses.Add(address);

                    foreach (int pos in floatingBits)
                    {
                        int adrCount = addresses.Count;
                        for (int i = 0; i < adrCount; i++)
                        {
                            long newAddress = FlipBit(addresses[i], (mask.Length - 1) - pos);
                            addresses.Add(newAddress);
                        }
                    }

                    for (int i = 0; i < addresses.Count; i++)
                    {
                        if (!memory.ContainsKey(addresses[i])) memory.Add(addresses[i], value);
                        else memory[addresses[i]] = value;
                    }
                }
            }

            long result = 0;
            foreach (long value in memory.Values)
            {
                result += value;
            }
            return result.ToString();
        }

        private long FlipBit(long n, int p)
        {
            return (n ^ ((long)1 << p));
        }
    }
}