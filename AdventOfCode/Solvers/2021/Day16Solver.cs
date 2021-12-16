using System.Text;

namespace AdventOfCode.Solvers.Year2021
{
    class Day16Solver : AbstractSolver
    {
        class Packet
        {
            readonly int Version;
            readonly int TypeID;
            readonly ulong Value;
            readonly List<Packet> SubPackets = new();

            public Packet(string bitString, out string remainingBitString)
            {
                Version = Convert.ToInt32(bitString[..3], 2);
                TypeID = Convert.ToInt32(bitString[3..6], 2);

                remainingBitString = bitString[6..];
                if (TypeID == 4)
                {
                    StringBuilder sb = new();
                    while (true)
                    {
                        _ = sb.Append(remainingBitString[1..5]);
                        if (remainingBitString[0] == '0')
                        {
                            remainingBitString = remainingBitString[5..];
                            break;
                        }
                        remainingBitString = remainingBitString[5..];
                    }
                    Value = Convert.ToUInt64(sb.ToString(), 2);
                }
                else
                {
                    if (remainingBitString[0] == '0')
                    {
                        int lengthInBits = Convert.ToInt32(remainingBitString[1..16], 2);

                        string subBitString = remainingBitString[16..(16 + lengthInBits)];
                        while (subBitString.Length > 0)
                        {
                            SubPackets.Add(new Packet(subBitString, out subBitString));
                        }
                        remainingBitString = remainingBitString[(16 + lengthInBits)..];
                    }
                    else
                    {
                        int numberOfSubPackets = Convert.ToInt32(remainingBitString[1..12], 2);
                        remainingBitString = remainingBitString[12..];

                        for (int n = 0; n < numberOfSubPackets; n++)
                        {
                            SubPackets.Add(new Packet(remainingBitString, out remainingBitString));
                        }
                    }
                }
            }

            public int GetVersionSum()
            {
                int versionSum = Version;
                foreach (Packet packet in SubPackets) versionSum += packet.GetVersionSum();
                return versionSum;
            }

            public ulong GetValue()
            {
                ulong value = 0;
                switch (TypeID)
                {
                    case 0:
                        foreach (Packet packet in SubPackets) value += packet.GetValue();
                        break;
                    case 1:
                        value = 1;
                        foreach (Packet packet in SubPackets) value *= packet.GetValue();
                        break;
                    case 2:
                        value = SubPackets.MinBy(p => p.GetValue())!.GetValue();
                        break;
                    case 3:
                        value = SubPackets.MaxBy(p => p.GetValue())!.GetValue();
                        break;
                    case 4:
                        value = Value;
                        break;
                    case 5:
                        value = (ulong)((SubPackets[0].GetValue() > SubPackets[1].GetValue())?1:0);
                        break;
                    case 6:
                        value = (ulong)((SubPackets[0].GetValue() < SubPackets[1].GetValue()) ? 1 : 0);
                        break;
                    case 7:
                        value = (ulong)((SubPackets[0].GetValue() == SubPackets[1].GetValue()) ? 1 : 0);
                        break;

                }
                return value;
            }
        }


        public override string Part1()
        {
            string bitString = "";

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    StringBuilder sb = new();
                    foreach (char c in line)
                    {
                        string bits = Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2);
                        for (int i = 4; i > bits.Length; i--) _ = sb.Append('0');
                        _ = sb.Append(bits);
                    }

                    bitString = sb.ToString();
                }
            }

            Packet packet = new(bitString, out _);
            return packet.GetVersionSum().ToString();
        }

        public override string Part2()
        {
            string bitString = "";

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    StringBuilder sb = new();
                    foreach (char c in line)
                    {
                        string bits = Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2);
                        for (int i = 4; i > bits.Length; i--) _ = sb.Append('0');
                        _ = sb.Append(bits);
                    }

                    bitString = sb.ToString();
                }
            }

            Packet packet = new(bitString, out _);
            return packet.GetValue().ToString();
        }
    }
}
