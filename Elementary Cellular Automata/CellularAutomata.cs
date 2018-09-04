﻿using System.Collections;

namespace Elementary_Cellular_Automata
{
    public class CellularAutomata
    {
        private uint _currentRow = 0;

        //Rule  determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private BitArray Rule { get; }

        //Stores initial seed data as well as all CA outputs
        private BitMatrix Data { get; }

        public CellularAutomata(uint iterations, BitArray rule, BitArray seedData)
        {
            Rule = rule;
            //Todo determine width based on screen size
            Data = new BitMatrix(10, iterations + 1);
            for (var i = 0; i < seedData.Count; i++)
            {
                Data[0, (uint) i] = seedData[i];
            }
        }

        public void Iterate()
        {
            Data[_currentRow, 0] = Data[0, 0];

            for (uint i = 1; i < Data.ColumnCount - 1; i++)
            {
                var previousBit = Data[_currentRow, i - 1];
                var currentBit = Data[_currentRow, i];
                var nextBit = Data[_currentRow, i + 1];

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

            Data[_currentRow, Data.ColumnCount] = Data[0, Data.ColumnCount];

            _currentRow++;
        }
    }
}