using System;

namespace CheckersLogic
{
    public class ManageGame
    {
        private readonly CheckersBoardGame m_Board;
        private CheckersPlayer m_CurrentPlayer;
        private CheckersPlayer m_OpponentPlayer;

        public ManageGame() { }

        public ManageGame(int i_BoardSize, CheckersPlayer.eType i_Type, string i_CurrentPlayerName, string i_OpponentPlayerName, int i_CurrentTotalScore, int i_OpponentTotalScore)
        {
            m_Board = new CheckersBoardGame(i_BoardSize);
            m_CurrentPlayer = new CheckersPlayer(CheckersPlayer.eType.Player, CheckersTool.eSymbols.PlayerX, m_Board, i_CurrentPlayerName, i_CurrentTotalScore);
            m_OpponentPlayer = new CheckersPlayer(i_Type, CheckersTool.eSymbols.PlayerO, m_Board, i_OpponentPlayerName, i_OpponentTotalScore);      
            buildPlayerStepsLists();
        }

        public CheckersBoardGame Board
        {
            get
            {
                return m_Board;
            }
        }

        public CheckersPlayer CurrentPlayer
        {
            get
            {
                return m_CurrentPlayer;
            }
            set
            {
                m_CurrentPlayer = value;
            }
        }

        public CheckersPlayer OpponentPlayer
        {
            get
            {
                return m_OpponentPlayer;
            }
            set
            {
                m_OpponentPlayer = value;
            }
        }

        public void RunGame(Step i_CurrentStep, CheckersPlayer.eType i_PlayerType, ref bool isAnotherJump)
        {
            CheckersTool currentTool;
            bool isOptionToJump = m_CurrentPlayer.CanJump(), isJumpStep = true;

            if (i_PlayerType == CheckersPlayer.eType.Player)
            {
                if (!m_CurrentPlayer.IsValidSquare(m_Board, i_CurrentStep.StepFrom))
                {
                    throw new Exception("The step is not valid - cant move opponent tool!!!");
                }
            }

            currentTool = m_CurrentPlayer.GetTool(i_CurrentStep.StepFrom, !isJumpStep, isOptionToJump);

            if (i_PlayerType == CheckersPlayer.eType.Computer)
            {
     
                i_CurrentStep = currentTool.GetPcStep();
            }

            if (isAnotherJump)
            {
                if (!i_CurrentStep.StepFrom.IsEqualPoint(m_CurrentPlayer.LastStep.StepTo) || !m_CurrentPlayer.LastStep.IsJumpStep)
                {
                    throw new Exception("The step is not valid - must continue the jump!!!");
                }
            }

            if (!m_CurrentPlayer.PerformStep(m_Board, currentTool, i_CurrentStep.StepTo, m_OpponentPlayer, ref isAnotherJump, isOptionToJump))
            {
                throw new Exception("The step is not valid!!!");
            }

            if (!isAnotherJump)
            {
                CheckersPlayer.SwapPlayers(ref m_CurrentPlayer, ref m_OpponentPlayer);
            }

            m_CurrentPlayer.BuildListsOfValidSteps(m_Board);
        }

        private void buildPlayerStepsLists()
        {
            m_CurrentPlayer.BuildListsOfValidSteps(m_Board);
        }

        public void IsWin()
        {
            string message;

            if(!m_CurrentPlayer.IsCheckersTools() || !m_CurrentPlayer.IsValidSteps())
            {
                message = string.Format("{0}'s win!", m_OpponentPlayer.PlayerName);
                throw new Exception(message);
            }
        }

        public void IsTie()
        {
            if (!m_CurrentPlayer.IsValidSteps() && !m_OpponentPlayer.IsValidSteps())
            {
                throw new Exception("Tie!");
            }
        }

        public void SwapPlayers()
        {
            CheckersPlayer.SwapPlayers(ref m_CurrentPlayer, ref m_OpponentPlayer);
        }
    }
}