﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersLogic
{
    public class CheckersPlayer
    {
        private const int k_PlayerNameMaxSize = 20, k_ToolScore = 1, k_KingToolScore = 4;

        public enum eType
        {
            Player,
            Computer
        }

        private eType m_PlayerType;
        private string m_PlayerName;
        private CheckersTool.eSymbols m_PlayerSymbol;
        private List<CheckersTool> m_PlayerTools = new List<CheckersTool>();
        private List<CheckersTool> m_ComputerToolsValidList = new List<CheckersTool>();
        private List<CheckersTool> m_ComputerToolsJumpList = new List<CheckersTool>();
        private Step m_LastStep;
        private int m_Score = 0;
        private int m_TotalScore;

        public CheckersPlayer() { }

        public CheckersPlayer(eType i_PlayerType, CheckersTool.eSymbols i_PlayerSymbol, CheckersBoardGame i_Board, string i_PlayerName, int i_TotalScore)
        {
            m_PlayerType = i_PlayerType;
            m_PlayerName = i_PlayerName;
            m_PlayerSymbol = i_PlayerSymbol;
            m_TotalScore = i_TotalScore;
            i_Board.InitBoardArray(this);
        }

        public eType PlayerType
        {
            get
            {
                return m_PlayerType;
            }
            set
            {
                m_PlayerType = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return m_PlayerName;
            }
            set
            {
                m_PlayerName = value;
            }
        }

        public CheckersTool.eSymbols PlayerSymbol
        {
            get
            {
                return m_PlayerSymbol;
            }
            set
            {
                m_PlayerSymbol = value;
            }
        }

        public List<CheckersTool> PlayerTools
        {
            get
            {
                return m_PlayerTools;
            }

            set
            {
                m_PlayerTools = value;
            }
        }

        public Step LastStep
        {
            get
            {
                return m_LastStep;
            }

            set
            {
                m_LastStep = value;
            }
        }

        public int Score
        {
            get
            {
                return m_Score;
            }

            set
            {
                m_Score = value;
            }
        }

        public int TotalScore
        {
            get
            {
                return m_TotalScore;
            }

            set
            {
                m_TotalScore = value;
            }
        }

        public static void SwapPlayers(ref CheckersPlayer i_CurrentPlayer, ref CheckersPlayer i_OpponentPlayer)
        {
            CheckersPlayer tempPlayer = i_CurrentPlayer;
            i_CurrentPlayer = i_OpponentPlayer;
            i_OpponentPlayer = tempPlayer;
        }

        public static bool IsValidName(string i_PlayerName)
        {
            return i_PlayerName.Length <= k_PlayerNameMaxSize && !i_PlayerName.Contains(" ");
        }

        public static bool IsValidPlayerType(eType i_PlayerType)
        {
            return i_PlayerType == eType.Computer || i_PlayerType == eType.Player;
        }

        public CheckersTool GetTool(Point i_Point, bool i_RemoveTool, bool i_IsOptionToJump)
        {
            CheckersTool returnTool;

            if (i_RemoveTool || m_PlayerType == eType.Player)
            {
                returnTool = new CheckersTool(m_PlayerSymbol, i_Point);
                foreach (CheckersTool currentTool in m_PlayerTools)
                {
                    if (i_Point.IsEqualPoint(currentTool.Point))
                    {
                        returnTool = currentTool;
                        break;
                    }
                }
            }
            else
            {
                returnTool = GetPcTool(i_IsOptionToJump);
            }

            return returnTool;
        }

        public void BuildListsOfValidSteps(CheckersBoardGame i_Board)
        {
            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                currentTool.BuildList(i_Board);
            }
        }

        public bool PerformStep(CheckersBoardGame i_Board, CheckersTool i_CurrentTool, Point i_TargetPoint, CheckersPlayer i_OpponentPlayer, ref bool i_IsAnotherJump, bool i_IsOptionToJump)
        {
            bool IsPerformStep = true;
            Step currentStep = new Step(i_CurrentTool.Point, i_TargetPoint);

            if (Step.FindStep(ref currentStep, i_CurrentTool.ListOfJumpsSteps) || Step.FindStep(ref currentStep, i_CurrentTool.ListOfRegularSteps))
            {
                if(i_IsOptionToJump != currentStep.IsJumpStep)
                {
                    throw new Exception("The step is not valid - you must to do a jump step!!!");
                }

                i_CurrentTool.ChangeToKing(i_Board, i_TargetPoint);
                i_Board.BoardArray[currentStep.StepTo.Row, currentStep.StepTo.Col] = i_CurrentTool.Symbol;
                i_Board.BoardArray[currentStep.StepFrom.Row, currentStep.StepFrom.Col] = CheckersTool.eSymbols.Empty;
                m_PlayerTools.Remove(i_CurrentTool);
                i_CurrentTool.Point = currentStep.StepTo;
                m_PlayerTools.Add(i_CurrentTool);
                if (currentStep.IsJumpStep)
                {
                    i_OpponentPlayer.removeToolFromListTool(currentStep, currentStep.IsJumpStep);
                    i_Board.BoardArray[currentStep.PointToEat.Row, currentStep.PointToEat.Col] = CheckersTool.eSymbols.Empty;
                    i_IsAnotherJump = i_CurrentTool.IsAnotherJump(i_Board);
                }

                m_LastStep = new Step(currentStep.StepFrom, currentStep.StepTo, currentStep.IsJumpStep);
            }
            else
            {
                IsPerformStep = false;
            }

            return IsPerformStep;
        }

        private void removeToolFromListTool(Step i_Step, bool i_RemoveTool)
        {
            bool isOptionToJump = true;
            Point PointToEat = new Point(i_Step.PointToEat.Row, i_Step.PointToEat.Col);
            CheckersTool ToolToRemove = GetTool(PointToEat, i_RemoveTool, !isOptionToJump);
            m_PlayerTools.Remove(ToolToRemove);
        }

        public void CalcScore()
        {
            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                if (currentTool.Symbol == m_PlayerSymbol)
                {
                    m_Score += k_ToolScore;
                }
                else
                {
                    m_Score += k_KingToolScore;
                }
            }
        }

        public CheckersTool GetPcTool(bool i_IsOptionToJump)
        {
            int randomIndex;
            CheckersTool returnTool;

            if (i_IsOptionToJump)
            {
                m_ComputerToolsJumpList = buildComputerJumpList();
                randomIndex = new Random().Next(0, m_ComputerToolsJumpList.Count - 1);
                returnTool = m_ComputerToolsJumpList[randomIndex];
            }
            else
            {
                m_ComputerToolsValidList = buildComputerValidList();
                randomIndex = new Random().Next(0, m_ComputerToolsValidList.Count - 1);
                returnTool = m_ComputerToolsValidList[randomIndex];
            }

            return returnTool;
        }

        public bool IsValidSquare(CheckersBoardGame i_Board, Point i_Point)
        {
            return i_Board.BoardArray[i_Point.Row, i_Point.Col] == m_PlayerSymbol || i_Board.BoardArray[i_Point.Row, i_Point.Col] == kingSymbol();
        }

        private CheckersTool.eSymbols kingSymbol()
        {
            return m_PlayerSymbol == CheckersTool.eSymbols.PlayerX ? CheckersTool.eSymbols.KingX : CheckersTool.eSymbols.KingO;
        }

        public bool CanJump()
        {
            bool canJump = true;

            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                if (currentTool.ListOfJumpsSteps.Count() != 0)
                {
                    canJump = true;
                    break;
                }
                else
                {
                    canJump = false;
                }
            }

            return canJump;
        }

        private List<CheckersTool> buildComputerJumpList()
        {
            List<CheckersTool> computerList = new List<CheckersTool>();

            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                if (currentTool.ListOfJumpsSteps.Count() != 0)
                {
                    computerList.Add(currentTool);
                }
            }

            return computerList;
        }

        private List<CheckersTool> buildComputerValidList()
        {
            List<CheckersTool> computerList = new List<CheckersTool>();

            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                if (currentTool.ListOfRegularSteps.Count() != 0)
                {
                    computerList.Add(currentTool);
                }
            }

            return computerList;
        }

        public bool IsValidSteps()
        {
            bool isValidSteps = true;

            foreach (CheckersTool currentTool in m_PlayerTools)
            {
                if (currentTool.ListOfJumpsSteps.Count() != 0 || currentTool.ListOfRegularSteps.Count() != 0)
                {
                    isValidSteps = true;
                    break;
                }
                else
                {
                    isValidSteps = false;
                }
            }

            return isValidSteps;
        }

        public bool IsCheckersTools()
        {
            return m_PlayerTools.Count() != 0;
        }
    }
}