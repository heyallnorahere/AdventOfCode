using System;

namespace AdventOfCode
{
    public static class Utilities
    {
        public static int RequestInput(string prompt, int? defaultInput = null)
        {
            Console.Write($"{prompt}: ");

            // verify that the user did not immediately hit enter
            string? inputString = Console.ReadLine();
            if (inputString == null || inputString.Length == 0)
            {
                if (defaultInput != null)
                {
                    return defaultInput.Value;
                }
                else
                {
                    throw new ArgumentException("Cannot parse the lack of input!");
                }
            }

            return int.Parse(inputString);
        }
    }
}
