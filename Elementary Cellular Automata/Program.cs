using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //Arguments should be entered as "rule seed [speed]", if no speed is entered then 100ms is used
        //If no arguments are entered then program asks user from input
        private static void Main(string[] input)
        {
            string rule = "";
            string seed = "";
            string speed = "";

            //If input contains 2 or 3 items use them for rule, seed and speed
            //used to automate setup
            if (input.Length > 1 && input.Length < 4)
            {
                rule = input[0];
                seed = input[1];
                //Speed is an optional argument, defaults to 100ms otherwise
                speed = input.Length == 3 ? input[2] : "100";
            }

            GetRule(rule);
            //Only find screen width if seed has not be passed in, if one has then its width will be used
            if (seed == "")
            {
                //Get console width after rule to allow time for the console to be correctly sized
                GetScreenWidth();
            }
            GetSeedData(seed);
            GetSpeed(speed);

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

        //Gets rule from user or from optional argument
        private static void GetRule(string rule = "")
        {
            //Rule has been passed in
            if (rule.Length != 0)
            {
                //Checks if string contain a positive integer and is smaller then max rule size
                if (uint.TryParse(rule, out uint ruleInt) && ruleInt < Math.Pow(2, Rule.Length))
                {
                    rule = Convert.ToString(ruleInt, 2).PadLeft(Rule.Length);
                }
                //Checks if rule is not a binary number
                else if (rule.Length != Rule.Length || !rule.All(b => b == '0' || b == '1'))
                {
                    throw new ArgumentException("Rule input is not an " + Rule.Length
                                              + "bit binary number or a decimal number that represents one", nameof(rule));
                }

                for (int i = 0; i < Rule.Length; i++)
                {
                    Rule[Rule.Length - i - 1] = rule[i] == '1';
                }
                return;
            }

            string[] options = {
                "Rule 102",
                "Rule 110",
                "Manual Rule Decimal",
                "Manual Rule Binary"
            };

            //request rule from user if rule has not been passed in
            int option = GetOptionInput("Select Rule", options, true);

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

        //Gets seed from user or from optional argument
        private static void GetSeedData(string seed = "")
        {
            //seed has been passed in
            if (seed.Length != 0)
            {
                int maxWidth = Console.WindowWidth - 1;

                //seed length must be smaller than screen width to prevent wrapping to next line
                if (seed.Length > maxWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(seed), "seed length must be smaller than screen width, " +
                                                                        "current screen width is " + maxWidth);
                }

                if (!seed.All(b => b == '0' || b == '1'))
                {
                    throw new ArgumentException("seed contains invalid chars, seed must only contain binary numbers", nameof(seed));
                }

                //create array the size of the seed and add seed values
                _iterationWidth = seed.Length;
                _seedData = new BitArray(seed.Length);
                for (int i = 0; i < _iterationWidth; i++)
                {
                    _seedData[i] = seed[i] == '1';
                }
                return;
            }

            _seedData = new BitArray(_iterationWidth);

            string[] options = {
                "Single Top Left",
                "Single Top Middle",
                "Single Top Right",
                "Custom Row"
            };

            //request seed from user if seed has not been passed in
            int option = GetOptionInput("Select Initial Row", options, true);

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
        private static void GetSpeed(string speed = "")
        {
            //speed has been passed in
            if (speed.Length != 0)
            {
                if (!int.TryParse(speed, out int speedInt) || speedInt < 1)
                {
                    throw new ArgumentException("Speed must be a positive integer", nameof(speed));
                }

                _drawSpeed = speedInt;
                return;
            }

            string[] options =
        {
                "Very Fast (25ms)",
                "Fast (50ms)",
                "Medium (100ms)",
                "Slow (150ms)",
                "Very Slow (200ms)"
            };

            //request speed from user if speed has not been passed in
            int option = GetOptionInput("Select Draw Speed", options, true);

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
        private static int GetOptionInput(string request, IReadOnlyList<string> options, bool allowClear = false)
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

                //options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                if (int.TryParse(Console.ReadLine(), out int input) && input > 0 && input - 1 < options.Count)
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