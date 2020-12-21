using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    Alergens2Recipes[alergen].Add(this);
                }
            }
        }

        List<Recipe> Recipes = new List<Recipe>();
        static Dictionary<string, HashSet<Recipe>> Alergens2Recipes = new Dictionary<string, HashSet<Recipe>>();

        Dictionary<string, string> Identified = new Dictionary<string, string>();

        public override string Part1()
        {
            using(var input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    Recipes.Add(new Recipe(input.ReadLine()));
                }
            }

            while (Identified.Count != Alergens2Recipes.Count) {
                foreach (string alergen in Alergens2Recipes.Keys)
                {
                    HashSet<string> ingredients = null;
                    foreach(Recipe recipe in Alergens2Recipes[alergen])
                    {
                        if (ingredients == null) ingredients = new HashSet<string>(recipe.Ingredients.Where(i => !Identified.ContainsKey(i)));
                        else
                        {
                            ingredients.RemoveWhere(r => !recipe.Ingredients.Contains(r));
                        }
                    }
                    if(ingredients.Count == 1)
                    {
                        Identified.Add(ingredients.First(), alergen);
                    }
                }
            }

            HashSet<string> noAlergens = new HashSet<string>();
            foreach(Recipe recipe in Recipes)
            {
                foreach(string ingredient in recipe.Ingredients)
                {
                    if (!Identified.ContainsKey(ingredient)) noAlergens.Add(ingredient);
                }
            }

            int result = 0;
            foreach(string ingredient in noAlergens)
            {
                result += Recipes.Where(r => r.Ingredients.Contains(ingredient)).Count();
            }

            return result.ToString();
        }

        public override string Part2()
        {
            string result = "";
            foreach(string ingredient in Identified.OrderBy(i => i.Value).Select(i => i.Key))
            {
                if (result != "") result += ",";
                result += ingredient;
            }
            return result;
        }
    }
}
