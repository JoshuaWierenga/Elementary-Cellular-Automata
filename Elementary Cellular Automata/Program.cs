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

        //Arguments should be entered as --rule, --seed and --speed followed by input
        //rule should be either a integer between 0 and 255 or an 8 digit binary number
        //seed should be "left", "middle", "right" or a integer, speed should be a integer
        //if only some inputs are entered, defaults are used, they are in order: 110, middle and 100
        //if no arguments are entered then program asks user for input
        private static void Main(string[] input)
        {
            if (input.Length != 0)
            {
                (string rule, string seed, string speed) = ProcessArguments(input);
                GetRule(rule);
                GetSeedData(seed);
                GetSpeed(speed);
            }
            else
            {
                GetRule();
                GetScreenWidth();
                //Seed array is only instantiated if seed argument was not passed in or default is not being used
                //if it is not instantiated here it will be in GetSeedData
                _seedData = new BitArray(_iterationWidth);
                GetSeedData();
                GetSpeed();
            }

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

        //Split arguments into vars, includes defaults if some are not passed in
        private static (string rule, string seed, string speed)
            ProcessArguments(IReadOnlyList<string> arguments)
        {
            //Defaults, used if only some arguments are given
            //Rule 110
            string rule = "110";
            //single cell in middle of screen
            string seed = "middle";
            //100ms
            string speed = "100";

            for (int i = 0; i < arguments.Count; i += 2)
            {
                string option = arguments[i];
                string argument = arguments[i + 1];

                //Sets var matching option to argument
                //if var accepts letters then change to lowercase
                switch (option)
                {
                    case "--rule":
                        rule = argument;
                        break;
                    case "--seed":
                        seed = argument.ToLower();
                        break;
                    case "--speed":
                        speed = argument;
                        break;
                    default:
                        throw new ArgumentException("Option is not valid", nameof(option));
                }
            }

            return (rule, seed, speed);
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
            string[] options = {
                "Rule 102",
                "Rule 110",
                "Manual Rule Decimal",
                "Manual Rule Binary"
            };

            string option;

            //Checks if input has been passed in
            if (rule == "")
            {
                option = options[GetOptionInput("Select Rule", options, true)];
            }
            //Checks if input is not a number
            else if (!int.TryParse(rule, out int ruleInt))
            {
                throw new ArgumentException("Rule must be a positive integer", nameof(rule));
            }
            //Checks if input is an decimal number
            else if (ruleInt >= 0 && ruleInt < Math.Pow(2, Rule.Length))
            {
                option = "Manual Rule Decimal";
            }
            //Checks if input is a binary number
            else if (rule.Length == 8 && rule.All(c => c == '0' || c == '1'))
            {
                option = "Manual Rule Binary";
            }
            else
            {
                throw new ArgumentException("Rule must be a positive integer", nameof(rule));
            }

            switch (option)
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
                    if (rule == "")
                    {
                        Console.Write("Rule: ");
                    }
                    while (true)
                    {
                        if (rule == "")
                        {
                            //Request rule from use if not passed in
                            rule = Console.ReadLine();
                        }

                        if (!uint.TryParse(rule, out uint inputNum) || inputNum > 255) continue;

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
                        //Checks if a rule was passed in, if so uses it otherwises asks user for rule bit
                        Rule[i] = rule != ""
                            ? rule[i] == '1'
                            : GetBinaryInput("Output for \"" + Convert.ToString(i, 2).PadLeft(3, '0') + '\"');
                    }
                    break;
            }
        }

        //Gets seed from user or from optional argument
        private static void GetSeedData(string seed = "")
        {
            string[] options = {
                "Single Top Left",
                "Single Top Middle",
                "Single Top Right",
                "Custom Row"
            };

            string option;

            if (seed != "")
            {
                //Check if seed is word option
                if (seed == "left" || seed == "middle" || seed == "right")
                {
                    //Was not run in main as arguments were given but word options need existing seed array  
                    GetScreenWidth();
                    _seedData = new BitArray(_iterationWidth);
                    //Find only option that contains seed word
                    option = options.First(o => o.ToLower().Contains(seed));
                }
                //Check if seed is in binary
                else if (seed.All(c => c == '0' || c == '1'))
                {
                    int maxWidth = Console.WindowWidth - 1;

                    //seed length must be smaller than screen width to prevent wrapping to next line
                    if (seed.Length > maxWidth)
                    {
                        throw new ArgumentOutOfRangeException(nameof(seed), "Seed was out of range");
                    }

                    _iterationWidth = seed.Length;
                    _seedData = new BitArray(seed.Length);
                    option = "Custom Row";
                }
                else
                {
                    throw new ArgumentException("Seed must be a binary bumber", nameof(seed));

                }
            }
            else
            {
                //request seed from user as seed has not been passed in
                option = options[GetOptionInput("Select Initial Row", options, true)];
            }

            //Uses option name so that adding more options or reordering options doesn't affect option detection
            switch (option)
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
                        //Checks if a seed was passed in, if so uses it otherwises askes user for seed
                        _seedData[i] = seed != ""
                            ? seed[i] == '1'
                            : _seedData[i] = GetBinaryInput("State of position " + i);
                    }
                    break;
            }
        }

        //Gets speed from user, speed controls time between draws
        private static void GetSpeed(string speed = "")
        {
            //Checks if speed has been passed in
            if (speed.Length != 0)
            {
                if (!int.TryParse(speed, out int speedInt) || speedInt < 0)
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