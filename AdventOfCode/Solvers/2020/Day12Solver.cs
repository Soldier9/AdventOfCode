namespace AdventOfCode.Solvers.Year2020
{
    class Day12Solver : AbstractSolver
    {
        readonly List<(char, int)> instructions = new();

        char facing = 'E';
        int shipX = 0;
        int shipY = 0;

        public override string Part1()
        {
            using (StreamReader input = File.OpenText(InputFile))
            {
                while (!input.EndOfStream)
                {
                    string line = input.ReadLine()!;
                    instructions.Add((line[0], int.Parse(line.Remove(0, 1))));
                }
            }


            foreach ((char action, int value) in instructions)
            {
                switch (action)
                {
                    case 'N': shipY += value; break;
                    case 'S': shipY -= value; break;
                    case 'E': shipX += value; break;
                    case 'W': shipX -= value; break;
                    case 'R': TurnRight(value); break;
                    case 'L': TurnLeft(value); break;
                    case 'F': MoveForward(value); break;
                }
            }

            return (Math.Abs(shipX) + Math.Abs(shipY)).ToString();
        }

        private void MoveForward(int value)
        {
            switch (facing)
            {
                case 'N': shipY += value; break;
                case 'S': shipY -= value; break;
                case 'E': shipX += value; break;
                case 'W': shipX -= value; break;
            }
        }

        private void TurnRight(int value)
        {
            int turns = (value / 90) % 4;
            for (int i = 0; i < turns; i++)
            {
                switch (facing)
                {
                    case 'E': facing = 'S'; break;
                    case 'S': facing = 'W'; break;
                    case 'W': facing = 'N'; break;
                    case 'N': facing = 'E'; break;
                }
            }
        }

        private void TurnLeft(int value)
        {
            int turns = (value / 90) % 4;
            for (int i = 0; i < turns; i++)
            {
                switch (facing)
                {
                    case 'E': facing = 'N'; break;
                    case 'S': facing = 'E'; break;
                    case 'W': facing = 'S'; break;
                    case 'N': facing = 'W'; break;
                }
            }
        }


        int waypointX = 10;
        int waypointY = 1;

        public override string Part2()
        {
            shipX = 0;
            shipY = 0;

            foreach ((char action, int value) in instructions)
            {
                switch (action)
                {
                    case 'N': waypointY += value; break;
                    case 'S': waypointY -= value; break;
                    case 'E': waypointX += value; break;
                    case 'W': waypointX -= value; break;
                    case 'R': RotateRight(value); break;
                    case 'L': RotateLeft(value); break;
                    case 'F': MoveForward2(value); break;
                }
            }

            return (Math.Abs(shipX) + Math.Abs(shipY)).ToString();
        }

        private void RotateLeft(int value)
        {
            int tmpX = waypointX - shipX;
            int tmpY = waypointY - shipY;

            int turns = (value / 90) % 4;
            for (int i = 0; i < turns; i++)
            {
                waypointX = -tmpY;
                waypointY = tmpX;
                tmpX = waypointX;
                tmpY = waypointY;
            }
            waypointX += shipX;
            waypointY += shipY;
        }

        private void RotateRight(int value)
        {
            int tmpX = waypointX - shipX;
            int tmpY = waypointY - shipY;

            int turns = (value / 90) % 4;
            for (int i = 0; i < turns; i++)
            {
                waypointX = tmpY;
                waypointY = -tmpX;
                tmpX = waypointX;
                tmpY = waypointY;
            }
            waypointX += shipX;
            waypointY += shipY;
        }

        private void MoveForward2(int value)
        {
            int moveX = (waypointX - shipX) * value;
            int moveY = (waypointY - shipY) * value;

            waypointX += moveX;
            waypointY += moveY;
            shipX += moveX;
            shipY += moveY;
        }
    }
}
