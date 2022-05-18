namespace CheckersLogic
{
    public class CheckersBoardGame
    {
        private enum eBoardSize
        {
            Small = 6,
            Medium = 8,
            Large = 10
        }

        private int m_BoardSize;
        private readonly CheckersTool.eSymbols[,] m_BoardArray;

        public CheckersBoardGame() { }

        public CheckersBoardGame(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_BoardArray = new CheckersTool.eSymbols[m_BoardSize, m_BoardSize];
            clearBoard();
        }

        public CheckersTool.eSymbols[,] BoardArray
        {
            get
            {
                return m_BoardArray;
            }
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

        public void InitBoardArray(CheckersPlayer i_Player)
        {
            int i = 0;

            if (i_Player.PlayerSymbol == CheckersTool.eSymbols.PlayerO)
            {
                orderBoardArray(i, m_BoardSize / 2 - 1, CheckersTool.eSymbols.PlayerO, i_Player);
            }
            else
            {
                i = m_BoardSize / 2 + 1;
                orderBoardArray(i, m_BoardSize, CheckersTool.eSymbols.PlayerX, i_Player);
            }
        }

        private void orderBoardArray(int i_Start, int i_End, CheckersTool.eSymbols i_Symbol, CheckersPlayer i_Player)
        {
            for (; i_Start < i_End; i_Start++)
            {
                if (i_Start % 2 == 0)
                {
                    initPlayerTool(i_Symbol, 1, i_Start, i_Player);
                }
                else
                {
                    initPlayerTool(i_Symbol, 0, i_Start, i_Player);
                }
            }
        }

        private void initPlayerTool(CheckersTool.eSymbols i_Symbol, int i_Index, int i_Row, CheckersPlayer i_Player)
        {
            for (int i = i_Index; i < m_BoardSize; i += 2)
            {
                m_BoardArray[i_Row, i] = i_Symbol;
                Point newPoint = new Point(i_Row, i);
                i_Player.PlayerTools.Add(new CheckersTool(i_Symbol, newPoint));
            }
        }

        private void clearBoard()
        {
            for (int i = 0; i < m_BoardSize; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    m_BoardArray[i, j] = CheckersTool.eSymbols.Empty;
                }
            }
        }

        public static bool IsValidSize(int i_BoardSize)
        {
            return i_BoardSize == (int)eBoardSize.Small || i_BoardSize == (int)eBoardSize.Medium || i_BoardSize == (int)eBoardSize.Large;
        }
    }
}