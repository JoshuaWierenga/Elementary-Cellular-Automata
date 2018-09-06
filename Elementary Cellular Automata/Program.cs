using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Elementary_Cellular_Automata
{
    internal static class Program
    {
        //Number of times to run CA
        private const int Iterations = 1000000;

        //Width of each iteration, MUST BE ODD
        private static int _iterationWidth;

        //Time between each draw
        private static int _drawSpeed;

        //Rule determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private static readonly BitArray Rule = new BitArray(8);

        //Intial Row
        private static BitArray _seedData;

        private static CellularAutomata _ca;

        private static void Main()
        {
            GetRule();
            //Get console width after rule to allow time for the console to be correctly sized
            GetScreenWidth();
            _seedData = new BitArray(_iterationWidth);
            GetSeedData();
            GetSpeed();

            _ca = new CellularAutomata(Iterations, (uint)_iterationWidth, Rule, _seedData);

            CellularAutomata.SetupConsole();

            for (int i = 0; i < Iterations; i++)
            {
                if (i % 2 == 1)
                {
                    //Sleep between each draw to prevent screen glitching when too many new lines
                    //as well as making output slow enough to be understood
                    Thread.Sleep(_drawSpeed);
                    _ca.DisplayRow();
                }

                _ca.Iterate();
            }
        }

        //Find maximum data width for current console size
        private static void GetScreenWidth()
        {
            int screenWidth = Console.WindowWidth;
            //Check if width is odd
            if (screenWidth % 2 == 1)
            {
                //Trying to display rows at exact width when console width is odd often goes off the edge of the screen
                //removing 2 solves this while keeping the number odd
                _iterationWidth = screenWidth - 2;
            }
            else
            {
                //For there to be a single middle position the number of positions must be odd
                _iterationWidth = screenWidth - 1;
            }
        }

        //Gets rule from user
        private static void GetRule()
        {
            string[] options = {
                "Rule 102",
                "Rule 110",
                "Manual Rule Decimal",
                "Manual Rule Binary"
            };

            uint option = GetOptionInput("Select Rule", options, true);

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
                //Allows entering rule as a decimal number
                case "Manual Rule Decimal":
                    Console.Write("Rule: ");
                    while (true)
                    {
                        string input = Console.ReadLine();
                        if (!uint.TryParse(input, out uint inputNum) || inputNum >= 255) continue;

                        //Convert input back to string but as bits then store each bit in rule array
                        //Inputs are padded to ensure they are all 8 bits long
                        string inputBits = Convert.ToString(inputNum, 2).PadLeft(Rule.Length, '0');
                        for (int i = 0; i < inputBits.Length; i++)
                        {
                            //Input is backwards to rule so store in reverse order
                            Rule[Rule.Length - i - 1] = inputBits[i] == '1';
                        }
                        break;
                    }
                    break;
                //Allows entering rule as binary numbers
                case "Manual Rule Binary":
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

            uint option = GetOptionInput("Select Initial Row", options, true);

            //Uses option name so that adding more options or reordering options doesn't affect option detection
            switch (options[option])
            {
                case "Single Top Left":
                    _seedData[0] = true;
                    break;

                case "Single Top Middle":
                    //Width / 2 will give one below middle but SeedData starts at 0 so middle is one lower
                    _seedData[_iterationWidth / 2] = true;
                    break;

                case "Single Top Right":
                    _seedData[_iterationWidth - 1] = true;
                    break;

                case "Custom Row":
                    for (int i = 0; i < _iterationWidth; i++)
                    {
                        _seedData[i] = GetBinaryInput("State of position " + i);
                    }
                    break;
            }
        }

        //Gets speed from user, speed controls time between draws
        private static void GetSpeed()
        {
            string[] options =
            {
                "Very Fast (25ms)",
                "Fast (50ms)",
                "Medium (100ms)",
                "Slow (150ms)",
                "Very Slow (200ms)"
            };

            uint option = GetOptionInput("Select Draw Speed", options, true);

            //Uses option name so that adding more options or reordering options doesn't affect option detection
            switch (options[option])
            {
                case "Very Fast (25ms)":
                    _drawSpeed = 25;
                    break;

                case "Fast (50ms)":
                    _drawSpeed = 50;
                    break;

                case "Medium (100ms)":
                    _drawSpeed = 100;
                    break;

                case "Slow (150ms)":
                    _drawSpeed = 100;
                    break;

                case "Very Slow (200ms)":
                    _drawSpeed = 200;
                    break;
            }
        }

        //Displays options to user and returns number of option selected
        private static uint GetOptionInput(string request, IReadOnlyList<string> options, bool allowClear = false)
        {
            while (true)
            {
                if (allowClear)
                {
                    Console.Clear();
                }

                Console.WriteLine(request + ":");

                for (int i = 1; i <= options.Count; i++)
                {
                    string option = options[i - 1];
                    Console.WriteLine(i + " : " + option);
                }

                uint.TryParse(Console.ReadLine(), out uint input);
                //options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                //if the number is 0 then it underflows and is not in range and so will repeat just like if input > options.length - 1
                if (input - 1 < options.Count)
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

                int.TryParse(Console.ReadLine(), out int input);
                if (input == 0 || input == 1)
                {
                    return Convert.ToBoolean(input);
                }
            }
        }
    }
}