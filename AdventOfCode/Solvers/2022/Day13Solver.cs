namespace AdventOfCode.Solvers.Year2022
{
    class Day13Solver : AbstractSolver
    {
        class Packet : IComparable<Packet>
        {
            string stringRepresentation;
            bool isParsed = false;

            int value = -1;
            List<Packet> packets = new List<Packet>();

            public Packet(string stream)
            {
                stringRepresentation = stream;
            }

            public void Parse()
            {
                if (isParsed) return;
                if (stringRepresentation.StartsWith('['))
                {
                    List<string> subPackets = stringRepresentation.Substring(1, stringRepresentation.Length - 2).Split(",").ToList<string>();
                    for (int i = 0; i < subPackets.Count; i++)
                    {
                        // Recombine split subpackets
                        while (subPackets[i].Count(s => s == '[') > subPackets[i].Count(s => s == ']'))
                        {
                            subPackets[i] += "," + subPackets[i + 1];
                            subPackets.RemoveAt(i + 1);
                        }
                        // Only keep subpackets with content
                        if (subPackets[i].Length == 0) continue;
                        packets.Add(new Packet(subPackets[i]));
                    }
                }
                else value = int.Parse(stringRepresentation);
                isParsed = true;
            }

            public int CompareTo(Packet? rightPacket)
            {
                this.Parse();
                rightPacket!.Parse();
                if (this.value > -1 && rightPacket.value > -1) return this.value - rightPacket.value;
                else if (this.value > -1) return (new Packet("[" + this.value + "]")).CompareTo(rightPacket);
                else if (rightPacket.value > -1) return this.CompareTo(new Packet("[" + rightPacket.value + "]"));

                for (int i = 0; i < Math.Max(this.packets.Count, rightPacket.packets.Count); i++)
                {
                    if (i == this.packets.Count) return -1;
                    if (i == rightPacket.packets.Count) return 1;

                    int res = this.packets[i].CompareTo(rightPacket.packets[i]);
                    if (res != 0) return res;
                }

                return 0;
            }

            public override string ToString() => this.stringRepresentation;
        }

        List<Packet> packets = new List<Packet>();
        public override string Part1()
        {
            int result = 0;

            using (StreamReader input = File.OpenText(InputFile))
            {
                string[] sets = input.ReadToEnd().Split("\r\n\r\n");
                int index = 0;
                foreach (string set in sets)
                {
                    index++;
                    Packet leftPacket = new Packet(set.Split("\r\n")[0]);
                    Packet rightPacket = new Packet(set.Split("\r\n")[1]);
                    packets.Add(leftPacket);
                    packets.Add(rightPacket);
                    if (leftPacket.CompareTo(rightPacket) < 0) result += index;
                }
            }
            return result.ToString();
        }


        public override string Part2()
        {
            packets.Add(new Packet("[[2]]"));
            packets.Add(new Packet("[[6]]"));

            int result = 1;
            packets.Sort();
            for (int i = 0; i < packets.Count; i++)
            {
                if (packets[i].ToString() == "[[2]]" || packets[i].ToString() == "[[6]]") result *= (i + 1);
            }

            return result.ToString();
        }
    }
}