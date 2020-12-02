using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2020
{
    class Day2Solver : AbstractSolver
    {
        class PassToTest
        {
            private int num1;
            private int num2;
            private char character;
            private string password;

            public PassToTest(int num1, int num2, char character, string password)
            {
                this.num1 = num1;
                this.num2 = num2;
                this.character = character;
                this.password = password;
            }

            public bool Test1()
            {
                int count = 0;
                foreach (char c in password) if (c == character) count++;
                if (count >= num1 && count <= num2) return true;
                return false;
            }

            public bool Test2()
            {
                if (password[num1 - 1] == character && password[num2 - 1] != character) return true;
                if (password[num1 - 1] != character && password[num2 - 1] == character) return true;
                return false;
            }
        }

        private List<PassToTest> passwordsToTest = new List<PassToTest>();
        private Regex parser = new Regex(@"(\d+)\-(\d+)\s(\w):\s(\w+)");

        public override string Part1()
        {
            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine();

                    var parsed = parser.Matches(line)[0].Groups;
                    passwordsToTest.Add(new PassToTest( int.Parse(parsed[1].Value), int.Parse(parsed[2].Value), parsed[3].Value[0], parsed[4].Value));
                }
            }

            int goodPasswords = 0;
            foreach (var password in passwordsToTest)
            {
                if (password.Test1()) goodPasswords++;
            }
            return goodPasswords.ToString();
        }

        public override string Part2()
        {
            int goodPasswords = 0;
            foreach (var password in passwordsToTest)
            {
                if (password.Test2()) goodPasswords++;
            }
            return goodPasswords.ToString();
        }
    }
}
