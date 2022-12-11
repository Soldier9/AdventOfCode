using System.Numerics;

namespace AdventOfCode.Solvers.Year2022
{
    class Day11Solver : AbstractSolver
    {
        class Monkey
        {
            public static List<Monkey> monkeys = new();
            public static int mod = 1;

            public int inspected = 0;

            Queue<BigInteger> items = new();
            char opOperator;
            string opArg;
            int testArg;
            int trueDest;
            int falseDest;

            public Monkey(List<int> items, char opOperator, string opArg, int testArg, int trueDest, int falseDest)
            {
                foreach (int item in items) this.items.Enqueue(item);
                this.opOperator = opOperator;
                this.opArg = opArg;
                this.testArg = testArg;
                this.trueDest = trueDest;
                this.falseDest = falseDest;

                mod = lcm(mod, testArg);
            }

            private static int lcm(int x, int y)
            {
                return (x * y) / gcd(x, y);
            }

            private static int gcd(int x, int y)
            {
                int result = Math.Min(x, y);
                while (result > 0)
                {
                    if (x % result == 0 && y % result == 0) break;
                    result--;
                }
                return result;
            }


            public void ReceiveItem(BigInteger item) => this.items.Enqueue(item);

            public void TakeTurn(bool part1)
            {
                while (items.Count > 0)
                {
                    BigInteger item = items.Dequeue();
                    BigInteger arg = 0;
                    if (opArg == "old") arg = item;
                    else arg = int.Parse(opArg);

                    switch (opOperator)
                    {
                        case '*': item = item * arg; break;
                        case '+': item = item + arg; break;
                    }
                    if (part1) item /= 3;

                    item %= mod;

                    if (item % testArg == 0) monkeys[trueDest].ReceiveItem(item);
                    else monkeys[falseDest].ReceiveItem(item);

                    inspected++;
                }
            }
        }


        public override string Part1()
        {
            string[] inputs = File.OpenText(InputFile).ReadToEnd().Split("\r\n\r\n");
            foreach (string input in inputs)
            {
                string[] props = input.Split("\r\n");
                List<int> items = props[1].Split(":")[1].Split(",").Select(i => int.Parse(i)).ToList<int>();
                char opOp = props[2].Split("= old ")[1][0];
                string opArg = props[2].Split(opOp)[1].Trim();
                int testArg = int.Parse(props[3].Split(" by ")[1]);
                int trueDest = int.Parse(props[4].Split("monkey ")[1]);
                int falseDest = int.Parse(props[5].Split("monkey ")[1]);
                Monkey.monkeys.Add(new Monkey(items, opOp, opArg, testArg, trueDest, falseDest));
            }

            for (int i = 0; i < 20; i++)
            {
                foreach (Monkey monkey in Monkey.monkeys) monkey.TakeTurn(true);
            }

            int result = 1;
            List<Monkey> ordered = Monkey.monkeys.OrderByDescending(m => m.inspected).ToList<Monkey>();
            for (int i = 0; i < 2; i++) result *= ordered[i].inspected;

            return result.ToString();
        }

        public override string Part2()
        {
            Monkey.monkeys.Clear();

            string[] inputs = File.OpenText(InputFile).ReadToEnd().Split("\r\n\r\n");
            foreach (string input in inputs)
            {
                string[] props = input.Split("\r\n");
                List<int> items = props[1].Split(":")[1].Split(",").Select(i => int.Parse(i)).ToList<int>();
                char opOp = props[2].Split("= old ")[1][0];
                string opArg = props[2].Split(opOp)[1].Trim();
                int testArg = int.Parse(props[3].Split(" by ")[1]);
                int trueDest = int.Parse(props[4].Split("monkey ")[1]);
                int falseDest = int.Parse(props[5].Split("monkey ")[1]);
                Monkey.monkeys.Add(new Monkey(items, opOp, opArg, testArg, trueDest, falseDest));
            }

            for (int i = 0; i < 10000; i++)
            {
                foreach (Monkey monkey in Monkey.monkeys) monkey.TakeTurn(false);
            }

            BigInteger result = 1;
            List<Monkey> ordered = Monkey.monkeys.OrderByDescending(m => m.inspected).ToList<Monkey>();
            for (int i = 0; i < 2; i++) result *= ordered[i].inspected;

            return result.ToString();
        }
    }
}
