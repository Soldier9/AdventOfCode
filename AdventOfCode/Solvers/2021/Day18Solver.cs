namespace AdventOfCode.Solvers.Year2021
{
    class Day18Solver : AbstractSolver
    {
        class SnailFishNumber
        {
            public int? Value;
            public SnailFishNumber? Left;
            public SnailFishNumber? Right;
            private readonly SnailFishNumber? Parent;

            public override string ToString()
            {
                if (Value != null) return Value.ToString()!;
                return "[" + Left!.ToString() + "," + Right!.ToString() + "]";
            }

            private SnailFishNumber() { }
            public SnailFishNumber(string number, SnailFishNumber? parent = null)
            {
                Parent = parent;

                int separator = 0;
                int openBrackets = 0;
                for (int n = 0; n < number.Length; n++)
                {
                    if (number[n] == ',' && openBrackets == 1)
                    {
                        separator = n;
                        break;
                    }
                    if (number[n] == '[') openBrackets++;
                    else if (number[n] == ']') openBrackets--;
                }

                if (separator == 0)
                {
                    Value = int.Parse(number);
                }
                else
                {
                    Left = new SnailFishNumber(number[1..separator], this);
                    Right = new SnailFishNumber(number[(separator + 1)..^1], this);
                }
            }

            public void Reduce()
            {
                while (true)
                {
                    if (ReduceExplode()) continue;
                    if (ReduceSplit()) continue;
                    break;
                }
            }

            private bool ReduceExplode(int nesting = 0)
            {
                if (nesting == 4 && Value == null)
                {
                    Explode();
                    return true;
                }
                else
                {
                    if (Left != null)
                    {
                        if (Left.ReduceExplode(nesting + 1)) return true;
                    }
                    if (Right != null)
                    {
                        if (Right.ReduceExplode(nesting + 1)) return true;
                    }
                }
                return false;
            }

            private void Explode()
            {
                SnailFishNumber? valueLeft = FindValueLeft();
                if (valueLeft != null) valueLeft.Value += Left!.Value;

                SnailFishNumber? valueRight = FindValueRight();
                if (valueRight != null) valueRight.Value += Right!.Value;

                Left = null;
                Right = null;
                Value = 0;
            }

            private SnailFishNumber? FindValueLeft(bool goDown = false)
            {
                if (goDown)
                {
                    if (Value != null) return this;
                    return Right!.FindValueLeft(true);
                }
                else
                {
                    if (Parent == null) return null;
                    if (Parent.Left == this)
                    {
                        return Parent.FindValueLeft();
                    }
                    else
                    {
                        return Parent.Left!.FindValueLeft(true);
                    }
                }
            }

            private SnailFishNumber? FindValueRight(bool goDown = false)
            {
                if (goDown)
                {
                    if (Value != null) return this;
                    return Left!.FindValueRight(true);
                }
                else
                {
                    if (Parent == null) return null;
                    if (Parent.Right == this)
                    {
                        return Parent.FindValueRight();
                    }
                    else
                    {
                        return Parent.Right!.FindValueRight(true);
                    }
                }
            }

            private bool ReduceSplit()
            {
                if (Value != null && Value > 9)
                {
                    Split();
                    return true;
                }
                else
                {
                    if (Left != null)
                    {
                        if (Left.ReduceSplit()) return true;
                    }
                    if (Right != null)
                    {
                        if (Right.ReduceSplit()) return true;
                    }
                }
                return false;
            }

            private void Split()
            {
                double value = (double)Value!;
                Value = null;
                Left = new SnailFishNumber(Math.Floor(value / 2).ToString(), this);
                Right = new SnailFishNumber(Math.Ceiling(value / 2).ToString(), this);
            }

            public SnailFishNumber Add(SnailFishNumber toAdd)
            {
                SnailFishNumber result = new("[" + this + "," + toAdd + "]"); // creates a deep copy!
                result.Reduce();
                return result;
            }

            public int Magnitude()
            {
                if (Value != null) return (int)Value!;
                return (Left!.Magnitude() * 3) + (Right!.Magnitude() * 2);
            }
        }

        public override string Part1()
        {
            List<SnailFishNumber> snailFishNumbers = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    snailFishNumbers.Add(new SnailFishNumber(input.ReadLine()!));
                }
            }

            SnailFishNumber result = snailFishNumbers[0];

            for (int n = 1; n < snailFishNumbers.Count; n++)
            {
                result = result.Add(snailFishNumbers[n]);
            }

            return result.Magnitude().ToString();
        }

        public override string Part2()
        {
            List<SnailFishNumber> snailFishNumbers = new();

            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    snailFishNumbers.Add(new SnailFishNumber(input.ReadLine()!));
                }
            }

            int result = 0;
            for (int n = 0; n < snailFishNumbers.Count; n++)
            {
                for (int m = 0; m < snailFishNumbers.Count; m++)
                {
                    if (n != m)
                    {
                        result = Math.Max(result, snailFishNumbers[n].Add(snailFishNumbers[m]).Magnitude());
                    }
                }
            }

            return result.ToString();
        }
    }
}
