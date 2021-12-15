namespace AdventOfCode.Solvers.Year2019
{
    class Day14Solver : AbstractSolver
    {
        class Reaction
        {
            public readonly static Dictionary<string, Reaction> AllReactions = new();

            public readonly string Result;
            public readonly long Amount;

            readonly Dictionary<string, long> Ingredients = new();
            readonly Dictionary<Reaction, long> LinkedIngredients = new();

            readonly Dictionary<string, long> LeftOvers = new();

            public Reaction(string result, long amount, Dictionary<string, long> ingredients)
            {
                Result = result;
                Amount = amount;
                Ingredients = ingredients;

                AllReactions.Add(Result, this);
            }

            public static void LinkReactions()
            {
                foreach (Reaction reaction in AllReactions.Values)
                {
                    foreach (KeyValuePair<string, long> ingredient in reaction.Ingredients)
                    {
                        reaction.LinkedIngredients.Add(AllReactions[ingredient.Key], ingredient.Value);
                    }
                }
            }

            public void ClearLeftovers()
            {
                LeftOvers.Clear();
            }

            public long GetOreRequired(long amountRequired = 1)
            {

                if (Result == "ORE") return amountRequired;
                long oreRequired = 0;
                if (LeftOvers.ContainsKey(Result))
                {
                    if (LeftOvers[Result] > amountRequired)
                    {
                        LeftOvers[Result] -= amountRequired;
                        return 0;
                    }
                    else if (LeftOvers[Result] == amountRequired)
                    {
                        _ = LeftOvers.Remove(Result);
                        return 0;
                    }
                    else
                    {
                        amountRequired -= LeftOvers[Result];
                        _ = LeftOvers.Remove(Result);
                    }
                }

                long multiplesNeeded = (long)Math.Ceiling(amountRequired / (double)Amount);
                foreach (KeyValuePair<Reaction, long> ingredient in LinkedIngredients)
                {
                    oreRequired += ingredient.Key.GetOreRequired(multiplesNeeded * ingredient.Value);
                }
                long leftOver = (multiplesNeeded * Amount - amountRequired);
                if (LeftOvers.ContainsKey(Result)) LeftOvers[Result] += leftOver;
                else if (leftOver > 0) LeftOvers.Add(Result, leftOver);

                return oreRequired;
            }
        }

        public override string Part1()
        {
            //Dictionary<Reaction, long> amountsNeeded = new();
            _ = new Reaction("ORE", 1, new Dictionary<string, long>());

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string[] splitLine = input.ReadLine()!.Split(new string[] { " => " }, StringSplitOptions.None);
                    string[] splitResult = splitLine[1].Split(' ');
                    Dictionary<string, long> ingredients = new();

                    foreach (string ingredient in splitLine[0].Split(new string[] { ", " }, StringSplitOptions.None))
                    {
                        string[] splitIngredient = ingredient.Split(' ');
                        ingredients.Add(splitIngredient[1], long.Parse(splitIngredient[0]));
                    }

                    _ = new Reaction(splitResult[1], long.Parse(splitResult[0]), ingredients);
                }
            }

            Reaction.LinkReactions();
            return Reaction.AllReactions["FUEL"].GetOreRequired().ToString();
        }

        public override string Part2()
        {
            long target = 1000000000000;
            Reaction fuelReaction = Reaction.AllReactions["FUEL"];
            long lastAttempt = 1;
            long lastResult;
            long nextAttempt = 1000000000000 / fuelReaction.GetOreRequired();
            long currentAttempt;

            while (true)
            {
                currentAttempt = nextAttempt;
                fuelReaction.ClearLeftovers();
                lastResult = fuelReaction.GetOreRequired(currentAttempt);
                if (currentAttempt + 1 == lastAttempt && lastResult <= target) break;

                long adjustMent = Math.Max(Math.Abs((currentAttempt - lastAttempt) / 2), 1);
                if (lastResult > target) nextAttempt -= adjustMent;
                else nextAttempt += adjustMent;
                lastAttempt = currentAttempt;
            }
            return currentAttempt.ToString();
        }
    }
}
