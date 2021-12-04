namespace AdventOfCode.Solvers.Year2019
{
    class Day4Solver : AbstractSolver
    {
        readonly int inputFrom = 136818;
        readonly int inputTo = 685979;
        //int inputFrom = 112233;
        //int inputTo = 112233;

        public override string Part1()
        {
            int goodPws = 0;

            for (int testPw = inputFrom; testPw <= inputTo; testPw++)
            {
                string testPwStr = testPw.ToString();
                char? prevChar = null;
                bool goodPw = false;
                foreach (char c in testPwStr)
                {
                    if (prevChar == null)
                    {
                        prevChar = c;
                        continue;
                    }
                    if (c < prevChar)
                    {
                        goodPw = false;
                        break;
                    }
                    if (c == prevChar) goodPw = true;
                    prevChar = c;
                }
                if (goodPw) goodPws++;
            }

            return goodPws.ToString();
        }

        public override string Part2()
        {
            int goodPws = 0;

            for (int testPw = inputFrom; testPw <= inputTo; testPw++)
            {
                string testPwStr = testPw.ToString();
                char? prevChar = null;
                bool goodPw = false;
                int eqGrpSize = 1;
                foreach (char c in testPwStr)
                {
                    if (prevChar == null)
                    {
                        prevChar = c;
                        continue;
                    }
                    if (c < prevChar)
                    {
                        goodPw = false;
                        eqGrpSize = 1; // GAH
                        break;
                    }
                    if (c != prevChar)
                    {
                        if (eqGrpSize == 2) goodPw = true;
                        eqGrpSize = 1;
                    }
                    if (c == prevChar)
                    {
                        eqGrpSize++;
                    }

                    prevChar = c;
                }
                if (goodPw || eqGrpSize == 2) goodPws++;
            }

            return goodPws.ToString();
        }
    }
}
