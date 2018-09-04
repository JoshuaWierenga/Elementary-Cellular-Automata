using System.Collections;

namespace Elementary_Cellular_Automata
{
    internal static class Program
    {
        //Number of times to run CA
        private const int Iterations = 10;
        //Rule  determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private static readonly BitArray Rule = new BitArray(8);
        //Todo determine width based on screen size
        //Stores initial seed data as well as all CA outputs
        private static readonly BitMatrix Data = new BitMatrix(50, Iterations + 1);

        static void Main(string[] args)
        {

        }
    }
}