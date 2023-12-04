using System.Numerics;
using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solvers.Year2023
{
    class Day4Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<(HashSet<int> winningNumbers, HashSet<int> numbers)> cards = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                Regex parseNumbers = new(@"\d+");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    string[] numbers = line.Split(':')[1].Split('|');

                    (HashSet<int> winningNumbers, HashSet<int> numbers) card = (new(), new());
                    foreach (Match number in parseNumbers.Matches(numbers[0]))
                    {
                        card.winningNumbers.Add(int.Parse(number.Value));
                    }
                    foreach (Match number in parseNumbers.Matches(numbers[1]))
                    {
                        card.numbers.Add(int.Parse(number.Value));
                    }
                    cards.Add(card);
                }
            }

            int result = 0;
            foreach ((HashSet<int> winningNumbers, HashSet<int> numbers) card in cards)
            {
                result += (int)Math.Pow(2, card.numbers.Where(num => card.winningNumbers.Contains(num)).Count() - 1);
            }
            return result.ToString();
        }


        public override string Part2()
        {
            Dictionary<int, (HashSet<int> winningNumbers, HashSet<int> numbers, int copies)> cards = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                int cardNum = 0;
                Regex parseNumbers = new(@"\d+");
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    string[] numbers = line.Split(':')[1].Split('|');

                    (HashSet<int> winningNumbers, HashSet<int> numbers, int copies) card = (new(), new(), 1);
                    foreach (Match number in parseNumbers.Matches(numbers[0]))
                    {
                        card.winningNumbers.Add(int.Parse(number.Value));
                    }
                    foreach (Match number in parseNumbers.Matches(numbers[1]))
                    {
                        card.numbers.Add(int.Parse(number.Value));
                    }
                    cards.Add(++cardNum, card);
                }
            }

            foreach (KeyValuePair<int, (HashSet<int> winningNumbers, HashSet<int> numbers, int copies)> card in cards)
            {
                int wins = card.Value.numbers.Where(num => card.Value.winningNumbers.Contains(num)).Count();
                for (int i = card.Key + 1; i <= card.Key + wins; i++)
                {
                    if (!cards.ContainsKey(i)) continue;
                    (HashSet<int> winningNumbers, HashSet<int> numbers, int copies) newCard = cards[i];
                    newCard.copies += card.Value.copies;
                    cards[i] = newCard;
                }
            }

            int result = cards.Sum(card => card.Value.copies);
            return result.ToString();
        }
    }
}
