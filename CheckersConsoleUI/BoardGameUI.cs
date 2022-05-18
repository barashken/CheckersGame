using System;
using CheckersLogic;

namespace CheckersConsoleUI
{
    public class BoardGameUI
    {
        private int m_BoardSize;

        public BoardGameUI() { }

        public BoardGameUI(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
        }

        public int BoardSize
        {
            get
            {
                return m_BoardSize;
            }
            set
            {
                m_BoardSize = value;
            }
        }

        public void PrintBoardGame(CheckersBoardGame i_BoardLogic)
        {
            char currentSmallLetter = ManageGameUI.k_SmallA;

            printFirstLineInBoard();
            for (int i = 0; i < m_BoardSize; i++)
            {
                printLineBounder();
                printLine(currentSmallLetter, i_BoardLogic.BoardArray, i);
                currentSmallLetter++;
            }

            printLineBounder();
        }

        private void printFirstLineInBoard()
        {
            string spaceAndLetter, spaces = "   ";
            char currentBigLetter = ManageGameUI.k_BigA;

            for (int i = 0; i < m_BoardSize; i++)
            {
                spaceAndLetter = spaces + currentBigLetter;
                currentBigLetter++;
                Console.Write(spaceAndLetter);
            }

            Console.WriteLine();
        }

        private void printLineBounder()
        {
            string lineBounder = "====";

            Console.Write(" ");
            for (int i = 0; i < m_BoardSize; i++)
            {
                Console.Write(lineBounder);
            }

            Console.Write("=");
            Console.WriteLine();
        }

        private void printLine(char i_CurrentSmallLetter, CheckersTool.eSymbols[,] i_Board, int i_Row)
        {
            string bounder = "| ", lineInBoard = i_CurrentSmallLetter.ToString();

            for (int i = 0; i < m_BoardSize; i++)
            {
                lineInBoard += bounder + ((char)i_Board[i_Row, i]).ToString() + " ";
            }

            lineInBoard += '|';
            Console.WriteLine(lineInBoard);
        }
    }
}