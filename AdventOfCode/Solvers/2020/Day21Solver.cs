namespace AdventOfCode.Solvers.Year2020
{
    class Day21Solver : AbstractSolver
    {
        class Recipe
        {
            public HashSet<string> Ingredients;
            public HashSet<string> Alergens;

            public Recipe(string input)
            {
                string[] line = input.Split(new string[] { "(contains " }, StringSplitOptions.None);
                Ingredients = new HashSet<string>(line[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                Alergens = new HashSet<string>(line[1].Replace(")", "").Split(new string[] { ", " }, StringSplitOptions.None));

                foreach (string alergen in Alergens)
                {
                    if (!Alergens2Recipes.ContainsKey(alergen)) Alergens2Recipes.Add(alergen, new HashSet<Recipe>());
                    _ = Alergens2Recipes[alergen].Add(this);
                }
            }
        }

        readonly List<Recipe> Recipes = new();
        static readonly Dictionary<string, HashSet<Recipe>> Alergens2Recipes = new();
        readonly Dictionary<string, string> Identified = new();

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Recipes.Add(new Recipe(input.ReadLine()!));
                }
            }

            while (Identified.Count != Alergens2Recipes.Count)
            {
                foreach (string alergen in Alergens2Recipes.Keys)
                {
                    HashSet<string>? ingredients = null;
                    foreach (Recipe recipe in Alergens2Recipes[alergen])
                    {
                        if (ingredients == null)
                        {
                            ingredients = new HashSet<string>(recipe.Ingredients.Where(i => !Identified.ContainsKey(i)));
                        }
                        else
                        {
                            _ = ingredients.RemoveWhere(r => !recipe.Ingredients.Contains(r));
                        }
                    }
                    if (ingredients!.Count == 1)
                    {
                        Identified.Add(ingredients.First(), alergen);
                    }
                }
            }

            HashSet<string> noAlergens = new();
            foreach (Recipe recipe in Recipes)
            {
                foreach (string ingredient in recipe.Ingredients)
                {
                    if (!Identified.ContainsKey(ingredient)) _ = noAlergens.Add(ingredient);
                }
            }

            int result = 0;
            foreach (string ingredient in noAlergens)
            {
                result += Recipes.Where(r => r.Ingredients.Contains(ingredient)).Count();
            }

            return result.ToString();
        }

        public override string Part2()
        {
            string result = "";
            foreach (string ingredient in Identified.OrderBy(i => i.Value).Select(i => i.Key))
            {
                if (result != "") result += ",";
                result += ingredient;
            }
            return result;
        }
    }
}
