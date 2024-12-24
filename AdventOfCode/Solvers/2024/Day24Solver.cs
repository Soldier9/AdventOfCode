using System.Text;

namespace AdventOfCode.Solvers.Year2024
{
    class Day24Solver : AbstractSolver
    {
        class Gate
        {
            public static Dictionary<string, Gate> Gates = [];

            string Input1Name;
            string Input2Name;

            Gate? Input1 = null;
            Gate? Input2 = null;
            bool FixedOutput = false;

            string Type = "";

            public Gate(string type, string input1, string input2, bool fixedOutput)
            {
                Type = type;
                Input1Name = input1;
                Input2Name = input2;
                FixedOutput = fixedOutput;
            }

            public bool GetOutput()
            {
                switch (Type)
                {
                    case "AND": return Input1!.GetOutput() & Input2!.GetOutput();
                    case "OR": return Input1!.GetOutput() | Input2!.GetOutput();
                    case "XOR": return Input1!.GetOutput() ^ Input2!.GetOutput();
                    default: return FixedOutput;
                }
            }

            public static void MatchInputs()
            {
                foreach (var gate in Gates.Values.Where(g => g.Type != ""))
                {
                    gate.Input1 = Gates[gate.Input1Name];
                    gate.Input2 = Gates[gate.Input2Name];
                }
            }
        }

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                bool parsingFixed = true;
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    if (parsingFixed)
                    {
                        if (line.Length == 0)
                        {
                            parsingFixed = false;
                            continue;
                        }
                        var splitLine = line.Split(':', StringSplitOptions.TrimEntries);
                        Gate.Gates.Add(splitLine[0], new("", "", "", splitLine[1] == "1"));
                    }
                    else
                    {
                        var splitLine = line.Split(' ');
                        Gate.Gates.Add(splitLine[4], new(splitLine[1], splitLine[0], splitLine[2], false));
                    }
                }
            }

            Gate.MatchInputs();

            
            StringBuilder zOutput = new();
            foreach(var gate in Gate.Gates.Where(g => g.Key.StartsWith("z")).OrderByDescending(g=>g.Key))
            {
                zOutput.Append(gate.Value.GetOutput() ? "1" : "0");
            }
            string result = Convert.ToInt64(zOutput.ToString(), 2).ToString();
            
            return result;
        }

        public override string Part2()
        {

            return "Perhaps tomorrow!";
        }
    }
}
