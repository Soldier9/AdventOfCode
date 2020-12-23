using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2020
{
    class Day23Solver : AbstractSolver
    {

        public override string Part1()
        {
            LinkedList<int> cups = new LinkedList<int>();

            using (var input = File.OpenText(InputFile))
            {
                foreach (char c in input.ReadLine())
                {
                    cups.AddLast(c - 48);
                }

            }

            LinkedListNode<int> currentCup = cups.First;
            for (int r = 0; r < 100; r++)
            {
                LinkedList<int> removedCups = new LinkedList<int>();
                for (int n = 0; n < 3; n++)
                {
                    LinkedListNode<int> cupToRemove = currentCup.Next ?? currentCup.List.First;
                    cups.Remove(cupToRemove);
                    removedCups.AddLast(cupToRemove);
                }

                int destLabel = currentCup.Value - 1;
                if (destLabel == 0) destLabel = 9;
                while (removedCups.Contains(destLabel))
                {
                    destLabel--;
                    if (destLabel == 0) destLabel = 9;
                }

                LinkedListNode<int> destCup = cups.First;
                while (destCup.Value != destLabel) destCup = destCup.Next ?? destCup.List.First;

                while (removedCups.Count > 0)
                {
                    LinkedListNode<int> cupToInsert = removedCups.First;
                    removedCups.RemoveFirst();
                    cups.AddAfter(destCup, cupToInsert);
                    destCup = cupToInsert;
                }

                currentCup = currentCup.Next ?? currentCup.List.First;
            }

            string result = "";
            while (currentCup.Value != 1) currentCup = currentCup.Next ?? currentCup.List.First;
            currentCup = currentCup.Next ?? currentCup.List.First;
            for (int n = 0; n < 8; n++)
            {
                result += currentCup.Value;
                currentCup = currentCup.Next ?? currentCup.List.First;
            }

            return result;
        }

        public override string Part2()
        {
            LinkedList<long> cups = new LinkedList<long>();
            Dictionary<long, LinkedListNode<long>> lookup = new Dictionary<long, LinkedListNode<long>>();

            using (var input = File.OpenText(InputFile))
            {
                foreach (char c in input.ReadLine())
                {
                    cups.AddLast(c - 48);
                    lookup.Add(cups.Last.Value, cups.Last);
                }
            }
            for(int i = 10; i < 1000001; i++)
            {
                cups.AddLast(i);
                lookup.Add(cups.Last.Value, cups.Last);
            }

            LinkedListNode<long> currentCup = cups.First;
            for (int r = 0; r < 10000000; r++)
            {
                LinkedList<long> removedCups = new LinkedList<long>();
                for (int n = 0; n < 3; n++)
                {
                    LinkedListNode<long> cupToRemove = currentCup.Next ?? currentCup.List.First;
                    cups.Remove(cupToRemove);
                    removedCups.AddLast(cupToRemove);
                }

                long destLabel = currentCup.Value - 1;
                if (destLabel == 0) destLabel = 1000000;
                while (removedCups.Contains(destLabel))
                {
                    destLabel--;
                    if (destLabel == 0) destLabel = 1000000;
                }

                LinkedListNode<long> destCup = lookup[destLabel];
                
                while (removedCups.Count > 0)
                {
                    LinkedListNode<long> cupToInsert = removedCups.First;
                    removedCups.RemoveFirst();
                    cups.AddAfter(destCup, cupToInsert);
                    destCup = cupToInsert;
                }

                currentCup = currentCup.Next ?? currentCup.List.First;
            }

            long result;
            currentCup = lookup[1].Next ?? lookup[1].List.First;
            result = currentCup.Value * (currentCup.Next ?? currentCup.List.First).Value;

            return result.ToString();
        }
    }
}
