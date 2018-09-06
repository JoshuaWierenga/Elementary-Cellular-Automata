using System;
using System.Collections;

namespace Elementary_Cellular_Automata
{
    public class CellularAutomata
    {
        private uint _currentRow;

        //Rule  determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private BitArray Rule { get; }

        //Stores initial seed data as well as all CA outputs
        private BitMatrix Data { get; }

        public CellularAutomata(uint iterations, uint iterationWidth, BitArray rule, BitArray seedData)
        {
            Rule = rule;
            Data = new BitMatrix(iterations + 1, iterationWidth);
            for (int i = 0; i < seedData.Count; i++)
            {
                Data[0, (uint)i] = seedData[i];
            }
        }

        public void Iterate()
        {
            //First bit cannot change
            Data[_currentRow, 0] = Data[0, 0];

            for (uint i = 1; i < Data.ColumnCount - 1; i++)
            {
                bool previousBit = Data[_currentRow, i - 1];
                bool currentBit = Data[_currentRow, i];
                bool nextBit = Data[_currentRow, i + 1];

                //000
                if (!previousBit && !currentBit && !nextBit)
                {
                    Data[_currentRow + 1, i] = Rule[0];
                }
                //001
                else if (!previousBit && !currentBit)
                {
                    Data[_currentRow + 1, i] = Rule[1];
                }
                //010
                else if (!previousBit && !nextBit)
                {
                    Data[_currentRow + 1, i] = Rule[2];
                }
                //011
                else if (!previousBit)
                {
                    Data[_currentRow + 1, i] = Rule[3];
                }
                //100
                else if (!currentBit && !nextBit)
                {
                    Data[_currentRow + 1, i] = Rule[4];
                }
                //101
                else if (!currentBit)
                {
                    Data[_currentRow + 1, i] = Rule[5];
                }
                //110
                else if (!nextBit)
                {
                    Data[_currentRow + 1, i] = Rule[6];
                }
                //111
                else
                {
                    Data[_currentRow + 1, i] = Rule[7];
                }
            }

            //Last bit cannot
            Data[_currentRow + 1, Data.ColumnCount - 1] = Data[0, Data.ColumnCount - 1];

            _currentRow++;
        }

        //Draws a row and the the row before it as squares by using the unicode block elements
        public void DisplayRow()
        {
            DisplayRow(_currentRow);
        }

        //Draws a row and the the row before it as squares by using the unicode block elements
        public void DisplayRow(uint row)
        {
            for (uint i = 0; i < Data.ColumnCount; i++)
            {
                bool b = Data[row - 1, i];
                if (b)
                {
                    //if both the last row and this row are 1 at i then draw 2 stacked squares
                    //else if just the top row then draw a square in the top half of the char
                    bool b1 = Data[row, i];
                    Console.Write(b1 ? '█' : '▀');
                }
                else
                {
                    //if this row is 1 at i then draw a square in the bottom half of the char
                    //else leave it black and just draw a space
                    bool b1 = Data[row, i];
                    Console.Write(b1 ? '▄' : ' ');
                }
            }

            Console.WriteLine();
        }

        //Sets up console for displaying rows
        public static void SetupConsole(ConsoleColor backgroundColour = ConsoleColor.White, ConsoleColor forgroundColour = ConsoleColor.Black)
        {
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = forgroundColour;
            Console.CursorVisible = false;
            Console.Clear();
        }
    }
}