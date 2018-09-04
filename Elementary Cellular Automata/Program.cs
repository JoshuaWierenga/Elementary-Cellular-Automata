using System.Collections;

namespace Elementary_Cellular_Automata
{
    internal static class Program
    {
        //Number of times to run CA
        private const int Iterations = 10;
        //TODO Allow user to input rule
        //Rule  determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private static readonly BitArray Rule = new BitArray(8);
        //TODO Allow user to input seed
        private static readonly BitArray SeedData = new BitArray(10);

        private static CellularAutomata _ca;

        static void Main(string[] args)
        {
            //Rule 110
            Rule[0] = false;
            Rule[1] = true;
            Rule[2] = true;
            Rule[3] = false;
            Rule[4] = true;
            Rule[5] = true;
            Rule[6] = true;
            Rule[7] = false;

            for (int i = 0; i < 10; i++)
            {
                SeedData[i] = i == 9;
            }

            _ca = new CellularAutomata(Iterations, Rule, SeedData);
        }
    }
}