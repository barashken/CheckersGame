using System;
using CheckersConsoleUI;
using CheckersLogic;

namespace CheckersGame
{
    class Program
    {
        private const string k_ContinueGame = "1";

        public static void Main()
        {
            ManageGameUI game = new ManageGameUI();

            while (game.RunGame() == k_ContinueGame)
            {
                game.Game = new ManageGame(game.BoardGame.BoardSize, game.GameType, game.Game.CurrentPlayer.PlayerName,
                                           game.Game.OpponentPlayer.PlayerName, game.Game.CurrentPlayer.TotalScore,
                                           game.Game.OpponentPlayer.TotalScore);
            }

            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine("Good Bye (:");
            Console.ReadLine();
        }
    }
}