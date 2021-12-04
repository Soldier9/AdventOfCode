using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Year2019
{
    class Day22Solver : AbstractSolver
    {
        class Card
        {
            public readonly int Value;

            public Card Previous;
            public Card Next;

            public Card(int value)
            {
                Value = value;
            }
        }

        List<Card> CreateDeck(int cards)
        {
            var deck = new List<Card>();
            for (var i = 0; i < cards; i++)
            {
                deck.Add(new Card(i));
            }
            LinkCards(deck);
            return deck;
        }

        void LinkCards(List<Card> deck)
        {
            for (var i = 0; i < deck.Count; i++)
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
            var newDeck = new List<Card>();
            var card = deck[deck.Count - 1];
            for (var i = 0; i < deck.Count; i++)
            {
                newDeck.Add(card);
                card = card.Previous;
            }
            LinkCards(newDeck);
            return newDeck;
        }

        List<Card> CutDeck(List<Card> deck, int n)
        {
            var newDeck = new List<Card>();
            var card = deck[(deck.Count + n) % deck.Count];
            for (var i = 0; i < deck.Count; i++)
            {
                newDeck.Add(card);
                card = card.Next;
            }
            return newDeck;
        }

        List<Card> DealWithIncrement(List<Card> deck, int increment)
        {
            var tmpDeck = new Dictionary<int, Card>();
            for (var i = 0; i < deck.Count; i++)
            {
                tmpDeck.Add((i * increment) % deck.Count, deck[i]);
            }
            var newDeck = tmpDeck.OrderBy(c => c.Key).Select(c => c.Value).ToList<Card>();
            LinkCards(newDeck);
            return newDeck;
        }

        public override string Part1()
        {
            var deck = CreateDeck(10007);

            using (var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    var line = input.ReadLine();
                    if (line.StartsWith("deal into new stack"))
                    {
                        deck = DealIntoNewStack(deck);
                    }
                    else if (line.StartsWith("cut"))
                    {
                        var value = int.Parse(line.Substring(4));
                        deck = CutDeck(deck, value);
                    }
                    else if (line.StartsWith("deal with increment "))
                    {
                        var value = int.Parse(line.Substring(20));
                        deck = DealWithIncrement(deck, value);
                    }
                }
            }

            for (var i = 0; i < deck.Count; i++)
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
