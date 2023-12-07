using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solvers.Year2023
{
    class Day7Solver : AbstractSolver
    {
        class Hand : IComparable<Hand>
        {
            bool Part2 = false;
            List<char> CardTypes = new List<char> { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

            string Cards;
            public int Bid;

            public Hand(string cards, int bid, bool part2 = false)
            {
                Cards = cards;
                Bid = bid;

                Part2 = part2;
                if (Part2) CardTypes = new List<char> { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };
            }

            private int TypeOfHand(string cardsOnHand)
            {
                HashSet<char> distinctCards = new(cardsOnHand);
                if (Part2 && distinctCards.Contains('J'))
                {
                    if (distinctCards.Count == 1) return 1; // This handles special case JJJJJ
                    
                    int bestHand = int.MaxValue;
                    foreach (char card in distinctCards.Where(c => c != 'J'))
                    {
                        int testHand = TypeOfHand(cardsOnHand.Replace('J', card));
                        if (testHand < bestHand) bestHand = testHand;
                    }
                    return bestHand;
                }

                if (distinctCards.Count == 1) return 1; // Five of a Kind
                else if (distinctCards.Count == 2)
                {
                    if (cardsOnHand.Where(c => c == distinctCards.First()).Count() == 1 || cardsOnHand.Where(c => c == distinctCards.First()).Count() == 4) return 2; // Four of a Kind
                    return 3; // Full House
                }
                else if (distinctCards.Count == 3)
                {
                    foreach (char card in distinctCards)
                    {
                        if (cardsOnHand.Where(c => c == card).Count() == 3) return 4; // Three of a Kind
                    }
                    return 5; // Two Pairs
                }
                else if (distinctCards.Count == 4) return 6; // Pair
                return 7; // High-Card
            }

            public int CompareTo(Hand? other)
            {
                if(this.TypeOfHand(this.Cards) == other!.TypeOfHand(other.Cards))
                {
                    for(int i = 0; i < 5; i++)
                    {
                        int comp = CardTypes.IndexOf(Cards[i]) - CardTypes.IndexOf(other.Cards[i]);
                        if (comp != 0) return comp;
                    }
                    return 0;
                }
                return other.TypeOfHand(other.Cards) - this.TypeOfHand(this.Cards);
            }

            public override string ToString()
            {
                return Cards;
            }
        }


        public override string Part1()
        {
            List<Hand> Hands = new List<Hand>();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ');
                    Hands.Add(new Hand(line[0], int.Parse(line[1])));
                }
            }

            Hands.Sort();

            int result = 0;
            int rank = 1;
            foreach(Hand hand in Hands) result += rank++ * hand.Bid;

            return result.ToString();
        }
        public override string Part2()
        {
            List<Hand> Hands = new List<Hand>();
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] line = input.ReadLine()!.Split(' ');
                    Hands.Add(new Hand(line[0], int.Parse(line[1]), true));
                }
            }

            Hands.Sort();

            int result = 0;
            int rank = 1;
            foreach (Hand hand in Hands) result += rank++ * hand.Bid;

            return result.ToString();
        }
    }
}
