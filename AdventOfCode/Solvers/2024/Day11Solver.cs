namespace AdventOfCode.Solvers.Year2024
{
    class Day11Solver : AbstractSolver
    {

        LinkedList<string> Stones = [];

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Stones = new LinkedList<string>(input.ReadLine()!.Split(' '));
            }

            for (int i = 0; i < 25; i++) Blink();

            return Stones.Count.ToString();
        }

        private void Blink()
        {
            LinkedListNode<string>? currentStone = Stones.First;
            do
            {
                if (currentStone!.Value == "0")
                {
                    currentStone.Value = "1";
                }
                else if (currentStone!.Value.Length % 2 == 0)
                {
                    Stones.AddBefore(currentStone, currentStone.Value.Substring(0, currentStone.Value.Length / 2));
                    currentStone.Value = long.Parse(currentStone.Value.Substring(currentStone.Value.Length / 2)).ToString();
                }
                else
                {
                    currentStone.Value = (long.Parse(currentStone.Value) * 2024).ToString();
                }
                currentStone = currentStone.Next;
            } while (currentStone != null);
        }

        public override string Part2()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                Stones = new LinkedList<string>(input.ReadLine()!.Split(' '));
            }

            long result = 0;
            foreach (string stone in Stones) result += Blink2(stone, 75);
            return result.ToString();
        }

        Dictionary<(string, int), long> CachedResults = [];
        private long Blink2(string stone, int blinks)
        {
            if (CachedResults.TryGetValue((stone, blinks), out long stones)) return stones;

            if (stone == "0")
            {
                if (blinks > 1) stones += Blink2("1", blinks - 1);
                else stones = 1;
            }
            else if (stone.Length % 2 == 0)
            {
                if (blinks > 1)
                {
                    stones += Blink2(stone.Substring(0, stone.Length / 2), blinks - 1);
                    stones += Blink2(long.Parse(stone.Substring(stone.Length / 2)).ToString(), blinks - 1);
                }
                else stones = 2;
            }
            else
            {
                if (blinks > 1) stones += Blink2((long.Parse(stone) * 2024).ToString(), blinks - 1);
                else stones = 1;
            }
            CachedResults.Add((stone, blinks), stones);
            return stones;
        }
    }
}
