using System;
using System.Text;
using CheckersLogic;

namespace CheckersConsoleUI
{
    public class ManageGameUI
    {
        private const int k_ScoreReset = 0;
        private const int k_StepLength = 5;
        private const string k_QuitString = "Q";
        private const string k_ComputerName = "Computer";
        public const char k_BigA = 'A';
        public const char k_SmallA = 'a';
        private const char k_StepSeperate = '>';

        private ManageGame m_Game;
        private readonly BoardGameUI m_BoardGame;
        private readonly CheckersPlayer.eType m_GameType;
        private readonly string m_FirstPlayerName;

        public ManageGameUI()
        {
            m_FirstPlayerName = getPlayerName();
            m_BoardGame = new BoardGameUI(getBoardSize());
            m_GameType = getGameType();
            m_Game = new ManageGame(m_BoardGame.BoardSize, m_GameType, m_FirstPlayerName, getPlayerName(), k_ScoreReset, k_ScoreReset);
        }
      
        public ManageGame Game
        {
            get
            {
                return m_Game;
            }
            set
            {
                m_Game = value;
            }
        }

        public BoardGameUI BoardGame
        {
            get
            {
                return m_BoardGame;
            }
        }

        public CheckersPlayer.eType GameType
        {
            get
            {
                return m_GameType;
            }
        }

        public string RunGame()
        {
            int countSteps = 0;
            Step inputStep = new Step();
            string inputString, outputMessage, continueGame;
            bool isAnotherJump = false;

            Ex02.ConsoleUtils.Screen.Clear();
            m_BoardGame.PrintBoardGame(m_Game.Board);
            inputString = getStep(countSteps);

            try
            {
                while (inputString != k_QuitString)
                {
                    if (m_Game.CurrentPlayer.PlayerType == CheckersPlayer.eType.Player)
                    {
                        inputStep = convertStringToStep(inputString);
                    }

                    try
                    {
                        m_Game.RunGame(inputStep, m_Game.CurrentPlayer.PlayerType, ref isAnotherJump);

                        if (m_Game.OpponentPlayer.PlayerType == CheckersPlayer.eType.Computer ||
                           (m_Game.OpponentPlayer.PlayerType == CheckersPlayer.eType.Player && isAnotherJump))
                        {
                            System.Threading.Thread.Sleep(3000);
                        }

                        Ex02.ConsoleUtils.Screen.Clear();
                        m_BoardGame.PrintBoardGame(m_Game.Board);
                        countSteps++;
                        printLastStep();
                    }
                    catch (Exception exception)
                    {
                        Ex02.ConsoleUtils.Screen.Clear();
                        m_BoardGame.PrintBoardGame(m_Game.Board);
                        if (countSteps != 0)
                        {
                            printLastStep();
                        }
                        Console.WriteLine(exception.Message);
                    }

                    m_Game.IsTie();
                    m_Game.IsWin();
                    inputString = getStep(countSteps);
                }

                outputMessage = string.Format("{0} quitted!", m_Game.CurrentPlayer.PlayerName);
            }
            catch (Exception exception)
            {
                outputMessage = string.Format("{0}", exception.Message);
            }
            
            m_Game.CurrentPlayer.CalcScore();
            m_Game.OpponentPlayer.CalcScore();
            m_Game.OpponentPlayer.TotalScore += m_Game.OpponentPlayer.Score - m_Game.CurrentPlayer.Score;
            outputMessage += string.Format(
@"
The score is:  
{0}: {1}
{2}: {3}", m_Game.CurrentPlayer.PlayerName, m_Game.CurrentPlayer.TotalScore, m_Game.OpponentPlayer.PlayerName, m_Game.OpponentPlayer.TotalScore);
            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine(outputMessage);
            continueGame = isWantToContinue();
            if(m_FirstPlayerName == m_Game.OpponentPlayer.PlayerName)
            {
                m_Game.SwapPlayers();
            }

            return continueGame;
        }

        private string getPlayerName()
        {
            string playerName = k_ComputerName;

            Ex02.ConsoleUtils.Screen.Clear();
            if (m_GameType == CheckersPlayer.eType.Player)
            {
                Console.WriteLine("Insert your name:");
                playerName = Console.ReadLine();
                while (!CheckersPlayer.IsValidName(playerName))
                {
                    Ex02.ConsoleUtils.Screen.Clear();
                    Console.WriteLine("Name is not valid, insert again:");
                    playerName = Console.ReadLine();
                }
            }

            return playerName;
        }

        private int getBoardSize()
        {
            int boardSize;
            bool isInt;

            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine("Insert board size (6 / 8 / 10):");
            isInt = int.TryParse(Console.ReadLine(), out boardSize);
            while (!(isInt && CheckersBoardGame.IsValidSize(boardSize)))
            {
                Ex02.ConsoleUtils.Screen.Clear();
                Console.WriteLine("Board size is not valid, insert again (6 / 8 / 10):");
                isInt = int.TryParse(Console.ReadLine(), out boardSize);
            }

            return boardSize;
        }

        private CheckersPlayer.eType getGameType()
        {
            CheckersPlayer.eType eGameType;
            int gameType;
            bool isInt;

            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine("Insert game type (0 for Human, 1 for Computer):");
            isInt = int.TryParse(Console.ReadLine(), out gameType);
            while (!isInt || !validGameType(gameType))
            {
                Ex02.ConsoleUtils.Screen.Clear();
                Console.WriteLine("Game type is not valid, insert again (0 for Human, 1 for Computer):");
                isInt = int.TryParse(Console.ReadLine(), out gameType);
            }

            eGameType = gameType == (int)CheckersPlayer.eType.Player ? CheckersPlayer.eType.Player : CheckersPlayer.eType.Computer;

            return eGameType;
        }

        private bool validGameType(int i_GameType)
        {
            return i_GameType == (int)CheckersPlayer.eType.Player || i_GameType == (int)CheckersPlayer.eType.Computer;
        }

        private string getStep(int i_CountSteps)
        {
            string stringStep = "";
            StringBuilder turnMessage = new StringBuilder();

            if (m_Game.CurrentPlayer.PlayerType == CheckersPlayer.eType.Player)
            {
                turnMessage.AppendFormat("{0}'s Turn ({1}): ", m_Game.CurrentPlayer.PlayerName, (char)m_Game.CurrentPlayer.PlayerSymbol);
                Console.Write(turnMessage);
                stringStep = Console.ReadLine();
                while (!(isValidStep(stringStep) || stringStep == k_QuitString))
                {
                    Ex02.ConsoleUtils.Screen.Clear();
                    m_BoardGame.PrintBoardGame(m_Game.Board);
                    if(i_CountSteps != 0)
                    {
                        printLastStep();
                    }
                    Console.WriteLine("Step is not valid! (use format Bs>Bs)");
                    Console.Write(turnMessage);
                    stringStep = Console.ReadLine();
                }
            }

            return stringStep;
        }

        private bool isValidStep(string i_StringStep)
        {
            bool isValid = true;

            if(i_StringStep.Length != k_StepLength)
            {
                isValid = false;
            }
            else
            {
                if(!(isValidSquare(i_StringStep[0], i_StringStep[1]) && i_StringStep[2] == k_StepSeperate && isValidSquare(i_StringStep[3], i_StringStep[4])))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool isValidSquare(char i_BigLetter, char i_SmallLetter)
        {
            return (char.IsUpper(i_BigLetter) && i_BigLetter - k_BigA <= m_BoardGame.BoardSize - 1 &&
                    char.IsLower(i_SmallLetter) && i_SmallLetter - k_SmallA <= m_BoardGame.BoardSize - 1);
        }

        private Step convertStringToStep(string i_StringStep)
        {
            Point stepFrom = new Point(i_StringStep[1] - k_SmallA, i_StringStep[0] - k_BigA);
            Point stepTo = new Point(i_StringStep[4] - k_SmallA, i_StringStep[3] - k_BigA);
            Step returnStep = new Step(stepFrom, stepTo);

            return returnStep;
        }

        private void printLastStep()
        {
            StringBuilder lastStepMessage = new StringBuilder();
            string lastStepString = convertStepToString(m_Game.OpponentPlayer.LastStep);

            lastStepMessage.AppendFormat("{0}'s move was ({1}): {2}", m_Game.OpponentPlayer.PlayerName, (char)m_Game.OpponentPlayer.PlayerSymbol, lastStepString);
            Console.WriteLine(lastStepMessage);
        }

        private string convertStepToString(Step i_Step)
        {
            string stepString = convertPointToString(i_Step.StepFrom) + k_StepSeperate.ToString() + convertPointToString(i_Step.StepTo);

            return stepString;
        }

        private string convertPointToString(Point i_Point)
        {
            char colLetter = (char)(k_BigA + i_Point.Col), rowLetter = (char)(k_SmallA + i_Point.Row);
            string pointString = colLetter.ToString() + rowLetter.ToString();

            return pointString;
        }

        private string isWantToContinue()
        {
            string isWantToContinue;

            Console.Write("Want to continue? If so, press 1, otherwise press any other key: ");
            isWantToContinue = Console.ReadLine();

            return isWantToContinue;
        }
    }
}