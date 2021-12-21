namespace AdventOfCode.Solvers.Year2021
{
    class Day21Solver : AbstractSolver
    {
        public override string Part1()
        {
            int player1Pos;
            int player2Pos;
            using (StreamReader input = File.OpenText(InputFile))
            {
                player1Pos = int.Parse(input.ReadLine()![^1].ToString());
                player2Pos = int.Parse(input.ReadLine()![^1].ToString());
            }

            int player1Points = 0;
            int player2Points = 0;
            int currentPlayer = 1;

            LinkedList<int> weightedDie = new();
            for (int i = 1; i <= 100; i++) _ = weightedDie.AddLast(i);
            LinkedListNode<int>? rolledDie = null;
            int dieRolls = 0;

            while (player1Points < 1000 && player2Points < 1000)
            {
                int roll = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (rolledDie == null) rolledDie = weightedDie.First;
                    else rolledDie = rolledDie.Next ?? weightedDie.First;
                    roll += rolledDie!.Value;
                    dieRolls++;
                }

                int currentPlayerPos = currentPlayer == 1 ? player1Pos : player2Pos;
                currentPlayerPos += roll;
                while (currentPlayerPos > 10) currentPlayerPos -= 10;

                int currentPlayerPoints = currentPlayer == 1 ? player1Points : player2Points;
                currentPlayerPoints += currentPlayerPos;

                if (currentPlayer == 1)
                {
                    player1Pos = currentPlayerPos;
                    player1Points = currentPlayerPoints;
                    currentPlayer = 2;
                }
                else
                {
                    player2Pos = currentPlayerPos;
                    player2Points = currentPlayerPoints;
                    currentPlayer = 1;
                }
            }

            return (Math.Min(player1Points, player2Points) * dieRolls).ToString();
        }

        readonly List<int> Rolls = new();
        readonly Dictionary<(int player1Pos, int player2Pos, int player1Score, int player2Score, int currentPlayer), (ulong player1, ulong player2)> cachedResults = new();
        public (ulong player1, ulong player2) GetWins(int player1Pos, int player2Pos, int player1Score, int player2Score, int currentPlayer)
        {
            if (player1Score >= 21) return (1, 0);
            if (player2Score >= 21) return (0, 1);
            if (cachedResults.ContainsKey((player1Pos, player2Pos, player1Score, player2Score, currentPlayer))) return cachedResults[(player1Pos, player2Pos, player1Score, player2Score, currentPlayer)];

            (ulong player1, ulong player2) wins = (0, 0);

            if (currentPlayer == 1)
            {
                foreach (int roll in Rolls)
                {
                    int newPlayer1Pos = player1Pos + roll;
                    while (newPlayer1Pos > 10) newPlayer1Pos -= 10;
                    int newPlayer1Score = player1Score + newPlayer1Pos;
                    (ulong player1, ulong player2) recursiveWins = GetWins(newPlayer1Pos, player2Pos, newPlayer1Score, player2Score, 2);
                    wins.player1 += recursiveWins.player1;
                    wins.player2 += recursiveWins.player2;
                }

                cachedResults.Add((player1Pos, player2Pos, player1Score, player2Score, currentPlayer), wins);
                return wins;
            }
            else
            {
                foreach (int roll in Rolls)
                {
                    int newPlayer2Pos = player2Pos + roll;
                    while (newPlayer2Pos > 10) newPlayer2Pos -= 10;
                    int newPlayer2Score = player2Score + newPlayer2Pos;
                    (ulong player1, ulong player2) recursiveWins = GetWins(player1Pos, newPlayer2Pos, player1Score, newPlayer2Score, 1);
                    wins.player1 += recursiveWins.player1;
                    wins.player2 += recursiveWins.player2;
                }

                cachedResults.Add((player1Pos, player2Pos, player1Score, player2Score, currentPlayer), wins);
                return wins;
            }
        }

        public override string Part2()
        {
            int player1Pos;
            int player2Pos;
            using (StreamReader input = File.OpenText(InputFile))
            {
                player1Pos = int.Parse(input.ReadLine()![^1].ToString());
                player2Pos = int.Parse(input.ReadLine()![^1].ToString());
            }

            for (int d1 = 1; d1 <= 3; d1++) for (int d2 = 1; d2 <= 3; d2++) for (int d3 = 1; d3 <= 3; d3++) Rolls.Add(d1 + d2 + d3);

            (ulong player1, ulong player2) wins = GetWins(player1Pos, player2Pos, 0, 0, 1);
            return Math.Max(wins.player1, wins.player2).ToString();
        }
    }
}
