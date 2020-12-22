using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2020
{
    class Day22Solver : AbstractSolver
    {
        public override string Part1()
        {
            Dictionary<int, LinkedList<int>> decks = new Dictionary<int, LinkedList<int>>();

            using (var input = File.OpenText(InputFile))
            {
                int currentPlayer = 0;
                string line = "";
                while (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    if (line.StartsWith("Player"))
                    {
                        currentPlayer = int.Parse(line.Substring(7, 1));
                        decks[currentPlayer] = new LinkedList<int>();
                    }
                    else if (line.Length == 0) continue;
                    else
                    {
                        decks[currentPlayer].AddLast(int.Parse(line));
                    }
                }
            }

            while (decks[1].Count > 0 && decks[2].Count > 0)
            {
                if (decks[1].First.Value > decks[2].First.Value)
                {
                    decks[1].AddLast(decks[1].First.Value);
                    decks[1].AddLast(decks[2].First.Value);
                }
                else
                {
                    decks[2].AddLast(decks[2].First.Value);
                    decks[2].AddLast(decks[1].First.Value);
                }
                decks[1].RemoveFirst();
                decks[2].RemoveFirst();
            }

            long result = 0;
            for (int i = 1; i < 3; i++)
            {
                int value = decks[i].Count;
                foreach (int card in decks[i])
                {
                    result += value * card;
                    value--;
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            Dictionary<int, LinkedList<int>> decks = new Dictionary<int, LinkedList<int>>();

            using (var input = File.OpenText(InputFile))
            {
                int currentPlayer = 0;
                string line = "";
                while (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    if (line.StartsWith("Player"))
                    {
                        currentPlayer = int.Parse(line.Substring(7, 1));
                        decks[currentPlayer] = new LinkedList<int>();
                    }
                    else if (line.Length == 0) continue;
                    else
                    {
                        decks[currentPlayer].AddLast(int.Parse(line));
                    }
                }
            }

            RecursiveCombat(decks);

            long result = 0;
            for (int i = 1; i < 3; i++)
            {
                int value = decks[i].Count;
                foreach (int card in decks[i])
                {
                    result += value * card;
                    value--;
                }
            }

            return result.ToString();
        }


        private int RecursiveCombat(Dictionary<int, LinkedList<int>> decks)
        {
            HashSet<int> previousRounds = new HashSet<int>();
            while (decks[1].Count > 0 && decks[2].Count > 0)
            {
                int currentRoundHash = GetSequenceHashCode<int>(decks[1].ToList());
                if (previousRounds.Contains(currentRoundHash)) return 1;
                previousRounds.Add(currentRoundHash);

                int card1 = decks[1].First.Value;
                int card2 = decks[2].First.Value;
                decks[1].RemoveFirst();
                decks[2].RemoveFirst();

                if (decks[1].Count >= card1 && decks[2].Count >= card2)
                {
                    Dictionary<int, LinkedList<int>> nextDeck = new Dictionary<int, LinkedList<int>>();
                    LinkedListNode<int> d1 = null;
                    LinkedListNode<int> d2 = null;
                    for (int n = 0; n < Math.Max(card1, card2); n++)
                    {
                        if (n == 0)
                        {
                            nextDeck[1] = new LinkedList<int>();
                            nextDeck[2] = new LinkedList<int>();
                            d1 = decks[1].First;
                            d2 = decks[2].First;
                        }

                        if (n < card1)
                        {
                            nextDeck[1].AddLast(d1.Value);
                            d1 = d1.Next;
                        }
                        if (n < card2)
                        {
                            nextDeck[2].AddLast(d2.Value);
                            d2 = d2.Next;
                        }
                    }

                    int recWinner = RecursiveCombat(nextDeck);
                    if (recWinner == 1)
                    {
                        decks[1].AddLast(card1);
                        decks[1].AddLast(card2);
                    }
                    else
                    {

                        decks[2].AddLast(card2);
                        decks[2].AddLast(card1);
                    }
                }
                else
                {
                    if (card1 > card2)
                    {
                        decks[1].AddLast(card1);
                        decks[1].AddLast(card2);
                    }
                    else
                    {
                        decks[2].AddLast(card2);
                        decks[2].AddLast(card1);
                    }
                }
            }

            if (decks[1].Count > 0) return 1;
            else return 2;
        }

        // Hash-function copied from StackExchange
        public static int GetSequenceHashCode<T>(IList<T> sequence)
        {
            const int seed = 487;
            const int modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
        }
    }
}
