namespace AdventOfCode.Solvers.Year2019
{
    class Day22Solver : AbstractSolver
    {
        class Card
        {
            public readonly int Value;

            public Card? Previous;
            public Card? Next;

            public Card(int value)
            {
                Value = value;
            }
        }

        static List<Card> CreateDeck(int cards)
        {
            List<Card> deck = new();
            for (int i = 0; i < cards; i++)
            {
                deck.Add(new Card(i));
            }
            LinkCards(deck);
            return deck;
        }

        static void LinkCards(List<Card> deck)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                if (i > 0)
                {
                    deck[i - 1].Next = deck[i];
                    deck[i].Previous = deck[i - 1];
                }
                if (i == deck.Count - 1)
                {
                    deck[i].Next = deck[0];
                    deck[0].Previous = deck[i];
                }
            }
        }

        List<Card> DealIntoNewStack(List<Card> deck)
        {
            List<Card> newDeck = new();
            Card card = deck[^1];
            for (int i = 0; i < deck.Count; i++)
            {
                newDeck.Add(card!);
                card = card.Previous!;
            }
            LinkCards(newDeck);
            return newDeck;
        }

        static List<Card> CutDeck(List<Card> deck, int n)
        {
            List<Card> newDeck = new();
            Card card = deck[(deck.Count + n) % deck.Count];
            for (int i = 0; i < deck.Count; i++)
            {
                newDeck.Add(card!);
                card = card.Next!;
            }
            return newDeck;
        }

        static List<Card> DealWithIncrement(List<Card> deck, int increment)
        {
            Dictionary<int, Card> tmpDeck = new();
            for (int i = 0; i < deck.Count; i++)
            {
                tmpDeck.Add((i * increment) % deck.Count, deck[i]);
            }
            List<Card> newDeck = tmpDeck.OrderBy(c => c.Key).Select(c => c.Value).ToList<Card>();
            LinkCards(newDeck);
            return newDeck;
        }

        public override string Part1()
        {
            List<Card> deck = CreateDeck(10007);

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (line.StartsWith("deal into new stack"))
                    {
                        deck = DealIntoNewStack(deck);
                    }
                    else if (line.StartsWith("cut"))
                    {
                        int value = int.Parse(line[4..]);
                        deck = CutDeck(deck, value);
                    }
                    else if (line.StartsWith("deal with increment "))
                    {
                        int value = int.Parse(line[20..]);
                        deck = DealWithIncrement(deck, value);
                    }
                }
            }

            for (int i = 0; i < deck.Count; i++)
            {
                if (deck[i].Value == 2019) return i.ToString();
            }

            return "No result found";
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
