using System;
using System.Collections;

namespace Elementary_Cellular_Automata
{
    internal static class Program
    {
        //Number of times to run CA
        private const int Iterations = 10;
        //Width of each iteration, MUST BE ODD
        private const int IterationWidth = 11;
        //Rule determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private static readonly BitArray Rule = new BitArray(8);
        //Intial Row
        private static readonly BitArray SeedData = new BitArray(IterationWidth);
        private static CellularAutomata _ca;

        private static void Main()
        {
            GetRule();
            GetSeedData();

            _ca = new CellularAutomata(Iterations, IterationWidth, Rule, SeedData);
        }

        //Gets rule from user
        private static void GetRule()
        {
            string[] options = {
                "Rule 102",
                "Rule 110",
                "Manual Rule"
            };

            var option = GetOptionInput("Select Rule", options, true);

            //Uses option name so that adding more options or reordering options doesn't affect option detection
            switch (options[option])
            {
                case "Rule 102":
                    Rule[0] = false;
                    Rule[1] = true;
                    Rule[2] = true;
                    Rule[3] = false;
                    Rule[4] = false;
                    Rule[5] = true;
                    Rule[6] = true;
                    Rule[7] = false;
                    break;
                case "Rule 110":
                    Rule[0] = false;
                    Rule[1] = true;
                    Rule[2] = true;
                    Rule[3] = true;
                    Rule[4] = false;
                    Rule[5] = true;
                    Rule[6] = true;
                    Rule[7] = false;
                    break;
                case "Manual Rule":
                    for (int i = 0; i < Rule.Length; i++)
                    {
                        //Gets output for specific inputs, inputs are found by converting i to a 3 digit binary number
                        Rule[i] = GetBinaryInput("Output for \"" + Convert.ToString(i, 2).PadLeft(3, '0') + '\"');
                    }
                    break;
            }
        }

        //Gets seed from user
        private static void GetSeedData()
        {
            string[] options = {
                "Single Top Left",
                "Single Top Middle",
                "Single Top Right",
                "Custom Row"
            };

            var option = GetOptionInput("Select Intitial Row", options, true);

            switch (options[option])
            {
                case "Single Top Left":
                    SeedData[0] = true;
                    break;
                case "Single Top Middle":
                    //Width / 2 will give one below middle but SeedData starts at 0 so middle is one lower
                    SeedData[IterationWidth / 2] = true;
                    break;
                case "Single Top Right":
                    SeedData[IterationWidth - 1] = true;
                    break;
                case "Custom Row":
                    break;
            }

        }

        //Displays options to user and returns number of option selected
        private static uint GetOptionInput(string request, string[] options, bool allowClear = false)
        {
            while (true)
            {
                if (allowClear)
                {
                    Console.Clear();
                }

                Console.WriteLine(request + ":");

                for (var i = 1; i <= options.Length; i++)
                {
                    var option = options[i - 1];
                    Console.WriteLine(i + " : " + option);
                }

                uint.TryParse(Console.ReadLine(), out var input);
                //options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                //if the number is 0 then it underflows and is not in range and so will repeat just like if input > options.length - 1
                if (input - 1 < options.Length)
                {
                    return input - 1;
                }
            }
        }

        //Displays request to user and returns user input of 0 or 1
        private static bool GetBinaryInput(string request, bool allowClear = false)
        {
            while (true)
            {
                if (allowClear)
                {
                    Console.Clear(); 
                }

                Console.Write(request + ": ");

                int.TryParse(Console.ReadLine(), out var input);
                if (input == 0 || input == 1)
                {
                    return Convert.ToBoolean(input);
                }
            }
        }
    }
}