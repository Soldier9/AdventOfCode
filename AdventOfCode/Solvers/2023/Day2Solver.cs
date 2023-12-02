using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solvers.Year2023
{
    class Day2Solver : AbstractSolver
    {
        Dictionary<int, Dictionary<string, int>> games = new();

        public override string Part1()
        {   
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    int id = int.Parse(line.Split(':')[0].Split(' ')[1]);
                    string[] grabs = line.Split(':')[1].Split(';');

                    games.Add(id, new());

                    foreach(string grab in grabs)
                    {
                        string[] colors = grab.Split(",");
                        foreach(string color in colors)
                        {
                            string cName = color.Trim().Split(' ')[1];
                            int cCount = int.Parse(color.Trim().Split(' ')[0]);

                            if (!games[id].ContainsKey(cName)) games[id].Add(cName, cCount);
                            else if (games[id][cName] < cCount) games[id][cName] = cCount;
                        }
                    }
                }
            }
            
            int result = 0;
            foreach(var game in games.Where(g => g.Value["red"] <= 12 && g.Value["green"] <= 13 && g.Value["blue"] <= 14))
            {
                result += game.Key;
            }
            return result.ToString();
        }

        public override string Part2()
        {
            int result = 0;
            foreach(var game in games)
            {
                result += (game.Value["red"] * game.Value["green"] * game.Value["blue"]);
            }
            return result.ToString();
        }
    }
}
