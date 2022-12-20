using System.Numerics;

namespace AdventOfCode.Solvers.Year2022
{
    class Day20Solver : AbstractSolver
    {
        class ElfLL
        {
            public static List<ElfLL> list = new();

            public ElfLL? prev;
            public ElfLL? next;
            public BigInteger num;
            public static ElfLL? zero = null;

            public ElfLL(BigInteger num)
            {
                this.num = num;
                if (this.num == 0) zero = this;

                if (list.Count > 0)
                {
                    prev = list[list.Count - 1];
                    next = list[0];
                    list[list.Count - 1].next = this;
                    list[0].prev = this;
                }
                list.Add(this);
            }

            public void moveForward(BigInteger places)
            {
                for (int i = 0; i < places; i++)
                {
                    ElfLL? ll0 = this.prev;
                    ElfLL? ll2 = this.next;
                    ElfLL? ll3 = this.next!.next;

                    ll0!.next = ll2;
                    ll2!.prev = ll0;
                    ll2.next = this;
                    this.prev = ll2;
                    this.next = ll3;
                    ll3!.prev = this;
                }
            }

            public void moveBack(BigInteger places)
            {
                for (int i = 0; i < places; i++)
                {
                    ElfLL? ll0 = this.prev!.prev;
                    ElfLL? ll1 = this.prev;
                    ElfLL? ll3 = this.next;

                    ll0!.next = this;
                    this.prev = ll0;
                    this.next = ll1;
                    ll1!.prev = this;
                    ll1.next = ll3;
                    ll3!.prev = ll1;
                }
            }

            public override string ToString() => num.ToString();
        }
        public override string Part1()
        {
            List<ElfLL> orgList = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    orgList.Add(new ElfLL(int.Parse(line)));
                }
            }

            ElfLL count = ElfLL.zero!;
            foreach (ElfLL org in orgList)
            {
                if (org.num > 0) org.moveForward((int)org.num);
                else if (org.num < 0) org.moveBack((int)org.num * -1);
            }

            count = ElfLL.zero!;
            BigInteger res = 0;
            for (int i = 1; i <= 3000; i++)
            {
                count = count.next!;
                if (i % 1000 == 0) res += count.num;
            }

            return res.ToString();
        }

        public override string Part2()
        {
            ElfLL.list = new();
            List<ElfLL> orgList = new();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    orgList.Add(new ElfLL(BigInteger.Parse(line) * 811589153));
                }
            }

            ElfLL count = ElfLL.zero!;
            for (int n = 0; n < 10; n++)
            {
                foreach (ElfLL org in orgList)
                {
                    if (org.num > 0) org.moveForward(org.num % (orgList.Count - 1));
                    else if (org.num < 0) org.moveBack((org.num * -1) % (orgList.Count - 1));
                }
            }

            count = ElfLL.zero!;
            BigInteger res = 0;
            for (int i = 1; i <= 3000; i++)
            {
                count = count.next!;
                if (i % 1000 == 0) res += count.num;
            }

            return res.ToString();
        }
    }
}
